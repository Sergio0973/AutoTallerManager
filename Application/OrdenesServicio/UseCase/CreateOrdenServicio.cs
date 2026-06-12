using System.Globalization;
using System.Text;
using Application.Abstractions;
using Application.Common.Exceptions;
using Application.Common.Security;
using Domain.Entities;
using Domain.ValueObjects.DetalleOrdenes;
using Domain.ValueObjects.LogInventarios;
using Domain.ValueObjects.OrdenServicios;
using FluentValidation;
using MediatR;

namespace Application.OrdenesServicio.UseCase;

public sealed record CreateOrdenServicio(
    int VehiculoId,
    string? Vin,
    int RecepcionistaId,
    int? EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateTime FechaIngreso,
    DateOnly? FechaEstimada,
    string? TipoServicio,
    int? TipoServicioId,
    int? MecanicoId,
    string? Observaciones,
    IReadOnlyList<CreateOrdenServicioRepuesto> Repuestos) : IRequest<CreateOrdenServicioResult>;

public sealed record CreateOrdenServicioRepuesto(int RepuestoId, int Cantidad);

public sealed record CreateOrdenServicioResult(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly FechaIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones,
    int? MecanicoId,
    int? TipoServicioId,
    string EstadoInicial,
    IReadOnlyList<RepuestoReservadoResult> RepuestosReservados);

public sealed record RepuestoReservadoResult(int RepuestoId, int Cantidad, decimal PrecioSnapshot, decimal Subtotal);

public sealed class CreateOrdenServicioValidator : AbstractValidator<CreateOrdenServicio>
{
    public CreateOrdenServicioValidator()
    {
        RuleFor(x => x.VehiculoId)
            .GreaterThan(0)
            .When(x => string.IsNullOrWhiteSpace(x.Vin));

        RuleFor(x => x.Vin)
            .NotEmpty()
            .When(x => x.VehiculoId <= 0);

        RuleFor(x => x.RecepcionistaId).GreaterThan(0);
        RuleFor(x => x.KilometrajeIngreso).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FechaIngreso).NotEmpty();
        RuleFor(x => x.Repuestos).NotNull();
        RuleForEach(x => x.Repuestos).ChildRules(repuesto =>
        {
            repuesto.RuleFor(x => x.RepuestoId).GreaterThan(0);
            repuesto.RuleFor(x => x.Cantidad).GreaterThan(0);
        });
    }
}

public sealed class CreateOrdenServicioHandler : IRequestHandler<CreateOrdenServicio, CreateOrdenServicioResult>
{
    private static readonly TimeSpan WorkShiftStart = new(8, 0, 0);
    private static readonly TimeSpan WorkShiftEnd = new(18, 0, 0);
    private static readonly TimeSpan BreakStart = new(12, 0, 0);
    private static readonly TimeSpan BreakEnd = new(13, 0, 0);

    private readonly IUnitOfWork _uow;
    private readonly IAuditoriaService _auditoriaService;

    public CreateOrdenServicioHandler(IUnitOfWork uow, IAuditoriaService auditoriaService)
    {
        _uow = uow;
        _auditoriaService = auditoriaService;
    }

    public async Task<CreateOrdenServicioResult> Handle(CreateOrdenServicio request, CancellationToken cancellationToken)
    {
        CreateOrdenServicioResult? result = null;

        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var vehiculo = await ResolveVehiculoAsync(request, ct);

            await UserRoleGuard.EnsureRecepcionistaAsync(_uow, request.RecepcionistaId, ct);

            var estado = await ResolveEstadoInicialAsync(request.EstadoId, ct);

            TipoServicio? tipoServicio = null;
            if (request.TipoServicioId.HasValue || !string.IsNullOrWhiteSpace(request.TipoServicio))
            {
                tipoServicio = await ResolveTipoServicioAsync(request.TipoServicioId, request.TipoServicio, ct);
            }

            if (await _uow.OrdenesServicio.HasActiveOrderForVehiculoAsync(vehiculo.Id, ct))
            {
                throw new InvalidOperationException($"El vehiculo {vehiculo.Id} ya posee una orden activa.");
            }

            if (request.CitaId.HasValue)
            {
                _ = await _uow.Citas.GetByIdAsync(request.CitaId.Value, ct)
                    ?? throw new KeyNotFoundException("Cita no encontrada.");
            }

            var fechaIngreso = DateOnly.FromDateTime(request.FechaIngreso);
            var fechaEstimada = request.FechaEstimada ?? CalculateEstimatedDate(request.FechaIngreso, tipoServicio);
            var duration = GetRequiredDuration(request.TipoServicio, tipoServicio);
            var fechaFin = request.FechaIngreso.Add(duration);

            if (request.MecanicoId.HasValue)
            {
                await UserRoleGuard.EnsureMecanicoAsync(_uow, request.MecanicoId.Value, ct);
                await EnsureMechanicAvailabilityAsync(request.MecanicoId.Value, request.FechaIngreso, fechaFin, ct);
            }

            var repuestosReservados = await ValidateStockAsync(request.Repuestos, ct);

            var orden = new OrdenServicio(
                vehiculo.Id,
                request.RecepcionistaId,
                estado.Id,
                request.CitaId,
                KilometrajeIngreso.Create(request.KilometrajeIngreso),
                fechaIngreso,
                fechaEstimada,
                string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesOrden.Create(request.Observaciones));

            await _uow.OrdenesServicio.AddAsync(orden, ct);
            await _uow.SaveChangesAsync(ct);

            if (tipoServicio is not null)
            {
                await _uow.OrdenesTiposServicio.AddAsync(new OrdenTipoServicio(orden.Id, tipoServicio.Id), ct);
            }

            if (request.MecanicoId.HasValue)
            {
                await _uow.OrdenesMecanicos.AddAsync(new OrdenMecanico(orden.Id, request.MecanicoId.Value, fechaIngreso), ct);
            }

            foreach (var item in repuestosReservados)
            {
                var repuesto = await _uow.Repuestos.GetByIdAsync(item.RepuestoId, ct)
                    ?? throw new KeyNotFoundException($"Repuesto {item.RepuestoId} no encontrado.");

                repuesto.DisminuirStock(item.Cantidad);

                await _uow.DetallesOrden.AddAsync(
                    new DetalleOrden(
                        orden.Id,
                        item.RepuestoId,
                        CantidadOrden.Create(item.Cantidad),
                        PrecioSnapshot.Create(item.PrecioSnapshot)),
                    ct);

                await _uow.Repuestos.UpdateAsync(repuesto, ct);
                await _uow.LogsInventario.AddAsync(
                    new LogInventario(
                        repuesto.Id,
                        request.MecanicoId ?? request.RecepcionistaId,
                        orden.Id,
                        null,
                        TipoMovimiento.Create("SALIDA_ORDEN"),
                        -item.Cantidad,
                        repuesto.StockActual,
                        MotivoMovimiento.Create("Reserva de inventario al crear orden de servicio.")),
                    ct);
            }

            await _uow.SaveChangesAsync(ct);

            result = new CreateOrdenServicioResult(
                orden.Id,
                orden.VehiculoId,
                orden.RecepcionistaId,
                orden.EstadoId,
                orden.CitaId,
                orden.KilometrajeIngreso.Value,
                orden.FechaIngreso,
                orden.FechaEstimada,
                orden.FechaEntregaReal,
                orden.Observaciones?.Value,
                request.MecanicoId,
                tipoServicio?.Id,
                estado.Nombre.Value,
                repuestosReservados);
        }, cancellationToken);

        await _auditoriaService.RegistrarAsync(
            request.RecepcionistaId,
            "OrdenServicio",
            result!.Id,
            "CREAR",
            null,
            new
            {
                result.Id,
                result.VehiculoId,
                result.RecepcionistaId,
                result.EstadoId,
                result.CitaId,
                result.KilometrajeIngreso,
                result.FechaIngreso,
                result.FechaEstimada,
                result.MecanicoId,
                result.TipoServicioId,
                result.RepuestosReservados
            },
            cancellationToken);

        return result;
    }

    private async Task<Vehiculo> ResolveVehiculoAsync(CreateOrdenServicio request, CancellationToken ct)
    {
        if (request.VehiculoId > 0)
        {
            return await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, ct)
                ?? throw new KeyNotFoundException("Vehiculo no encontrado.");
        }

        var vehiculos = await _uow.Vehiculos.GetAllAsync(ct);
        return vehiculos.FirstOrDefault(v => string.Equals(v.Vin.Value, request.Vin, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");
    }

    private async Task<EstadoOrden> ResolveEstadoInicialAsync(int? estadoId, CancellationToken ct)
    {
        if (estadoId.HasValue && estadoId.Value > 0)
        {
            return await _uow.EstadosOrden.GetByIdAsync(estadoId.Value, ct)
                ?? throw new KeyNotFoundException("Estado de orden no encontrado.");
        }

        var estados = await _uow.EstadosOrden.GetAllAsync(ct);
        var estado = estados.FirstOrDefault(e =>
            IsNamed(e.Nombre.Value, "Abierta") ||
            IsNamed(e.Nombre.Value, "Activa") ||
            IsNamed(e.Nombre.Value, "Agendada") ||
            IsNamed(e.Nombre.Value, "Recibida"));

        return estado ?? estados.OrderBy(e => e.Id).FirstOrDefault()
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");
    }

    private async Task<TipoServicio> ResolveTipoServicioAsync(int? tipoServicioId, string? nombre, CancellationToken ct)
    {
        if (tipoServicioId.HasValue && tipoServicioId.Value > 0)
        {
            return await _uow.TiposServicio.GetByIdAsync(tipoServicioId.Value, ct)
                ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");
        }

        var tipos = await _uow.TiposServicio.GetAllAsync(ct);
        var normalized = Normalize(nombre);
        return tipos.FirstOrDefault(t => Normalize(t.Nombre.Value) == normalized)
            ?? tipos.FirstOrDefault(t => IsCompatibleTipoServicio(normalized, Normalize(t.Nombre.Value)))
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");
    }

    private async Task EnsureMechanicAvailabilityAsync(int mecanicoId, DateTime start, DateTime end, CancellationToken ct)
    {
        if (end <= start)
        {
            throw new ArgumentException("La fecha final estimada debe ser mayor a la fecha de ingreso.");
        }

        if (!InWorkShift(start, end, WorkShiftStart, WorkShiftEnd))
        {
            throw new InvalidOperationException("La franja propuesta esta fuera de la jornada laboral del taller.");
        }

        if (IntersectsBreak(start, end, BreakStart, BreakEnd))
        {
            throw new InvalidOperationException("La franja propuesta se cruza con la pausa operativa del taller.");
        }

        var tareas = await _uow.TareasMecanicos.GetByMecanicoIdAsync(mecanicoId, ct);
        foreach (var tarea in tareas.Where(t => t.FechaInicio.HasValue && t.FechaFin.HasValue))
        {
            var orden = await _uow.OrdenesServicio.GetByIdAsync(tarea.OrdenId, ct);
            if (orden is null || !await IsActiveStatusAsync(orden.EstadoId, ct))
            {
                continue;
            }

            if (Overlaps(start, end, tarea.FechaInicio!.Value, tarea.FechaFin!.Value))
            {
                throw new InvalidOperationException($"Mecanico {mecanicoId} no disponible en la fecha {start:yyyy-MM-dd HH:mm}.");
            }
        }

        var asignaciones = await _uow.OrdenesMecanicos.GetByMecanicoIdAsync(mecanicoId, ct);
        foreach (var asignacion in asignaciones)
        {
            var orden = await _uow.OrdenesServicio.GetByIdAsync(asignacion.OrdenId, ct);
            if (orden is null || !await IsActiveStatusAsync(orden.EstadoId, ct))
            {
                continue;
            }

            var tareasOrden = await _uow.TareasMecanicos.GetByOrdenIdAsync(orden.Id, ct);
            if (tareasOrden.Any(t => t.FechaInicio.HasValue && t.FechaFin.HasValue))
            {
                continue;
            }

            if (orden.FechaIngreso == DateOnly.FromDateTime(start))
            {
                throw new InvalidOperationException($"Mecanico {mecanicoId} no disponible en la fecha {start:yyyy-MM-dd}.");
            }
        }
    }

    private async Task<IReadOnlyList<RepuestoReservadoResult>> ValidateStockAsync(
        IReadOnlyList<CreateOrdenServicioRepuesto> items,
        CancellationToken ct)
    {
        var insufficient = new List<StockInsuficienteItem>();
        var reserved = new List<RepuestoReservadoResult>();

        foreach (var group in items.GroupBy(i => i.RepuestoId))
        {
            var solicitado = group.Sum(i => i.Cantidad);
            var repuesto = await _uow.Repuestos.GetByIdAsync(group.Key, ct)
                ?? throw new KeyNotFoundException($"Repuesto {group.Key} no encontrado.");

            if (!repuesto.Activo || repuesto.StockActual < solicitado)
            {
                insufficient.Add(new StockInsuficienteItem(repuesto.Id, solicitado, repuesto.Activo ? repuesto.StockActual : 0));
                continue;
            }

            reserved.Add(new RepuestoReservadoResult(
                repuesto.Id,
                solicitado,
                repuesto.PrecioUnitario.Value,
                solicitado * repuesto.PrecioUnitario.Value));
        }

        if (insufficient.Count > 0)
        {
            throw new StockInsuficienteException(insufficient);
        }

        return reserved;
    }

    private async Task<bool> IsActiveStatusAsync(int estadoId, CancellationToken ct)
    {
        var estado = await _uow.EstadosOrden.GetByIdAsync(estadoId, ct);
        if (estado is null)
        {
            return false;
        }

        return IsActiveStatus(estado.Nombre.Value);
    }

    private static DateOnly CalculateEstimatedDate(DateTime fechaIngreso, TipoServicio? tipoServicio)
    {
        var days = tipoServicio?.DiasEstimados.Value ?? 1;
        if (days < 0)
        {
            days = 0;
        }

        var date = DateOnly.FromDateTime(fechaIngreso);
        while (days > 0)
        {
            date = date.AddDays(1);
            if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                days--;
            }
        }

        return date;
    }

    private static TimeSpan GetRequiredDuration(string? requestedType, TipoServicio? tipoServicio)
    {
        var name = Normalize(requestedType ?? tipoServicio?.Nombre.Value);

        if (name.Contains("MAYOR", StringComparison.OrdinalIgnoreCase))
        {
            return TimeSpan.FromHours(3);
        }

        if (name.Contains("DIAGNOSTICO", StringComparison.OrdinalIgnoreCase))
        {
            return TimeSpan.FromHours(1);
        }

        return TimeSpan.FromHours(1);
    }

    private static bool IsActiveStatus(string estado)
    {
        var normalized = Normalize(estado);
        return normalized is "ABIERTA" or "ACTIVA" or "ENPROCESO" or "AGENDADA" or "RECIBIDA" or "DIAGNOSTICO" or "REPARACION" or "PENDIENTE";
    }

    private static bool IsNamed(string value, string expected) => Normalize(value) == Normalize(expected);

    private static bool IsCompatibleTipoServicio(string requested, string stored)
    {
        if (string.IsNullOrWhiteSpace(requested) || string.IsNullOrWhiteSpace(stored))
        {
            return false;
        }

        return (requested.Contains("MANTENIMIENTO", StringComparison.OrdinalIgnoreCase) &&
                stored.Contains("MANTENIMIENTO", StringComparison.OrdinalIgnoreCase)) ||
            (requested.Contains("DIAGNOSTICO", StringComparison.OrdinalIgnoreCase) &&
                stored.Contains("DIAGNOSTICO", StringComparison.OrdinalIgnoreCase)) ||
            (requested.Contains("REPARACION", StringComparison.OrdinalIgnoreCase) &&
                stored.Contains("REPARACION", StringComparison.OrdinalIgnoreCase));
    }

    private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd) =>
        aStart < bEnd && aEnd > bStart;

    private static bool InWorkShift(DateTime start, DateTime end, TimeSpan dayStart, TimeSpan dayEnd) =>
        start.TimeOfDay >= dayStart && end.TimeOfDay <= dayEnd && start.Date == end.Date;

    private static bool IntersectsBreak(DateTime start, DateTime end, TimeSpan breakStart, TimeSpan breakEnd) =>
        start.TimeOfDay < breakEnd && end.TimeOfDay > breakStart;

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark &&
                !char.IsWhiteSpace(character) &&
                character != '_' &&
                character != '-')
            {
                builder.Append(char.ToUpperInvariant(character));
            }
        }

        return builder.ToString();
    }
}

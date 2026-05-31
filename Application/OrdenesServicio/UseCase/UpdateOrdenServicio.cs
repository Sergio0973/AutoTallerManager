using Application.Abstractions;
using Application.Common.Security;
using Domain.ValueObjects.OrdenServicios;
using FluentValidation;
using MediatR;

namespace Application.OrdenesServicio.UseCase;

public sealed record UpdateOrdenServicio(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones) : IRequest;

public sealed class UpdateOrdenServicioValidator : AbstractValidator<UpdateOrdenServicio>
{
    public UpdateOrdenServicioValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.RecepcionistaId).GreaterThan(0);
        RuleFor(x => x.EstadoId).GreaterThan(0);
        RuleFor(x => x.KilometrajeIngreso).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateOrdenServicioHandler : IRequestHandler<UpdateOrdenServicio>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditoriaService _auditoriaService;

    public UpdateOrdenServicioHandler(IUnitOfWork uow, IAuditoriaService auditoriaService)
    {
        _uow = uow;
        _auditoriaService = auditoriaService;
    }

    public async Task Handle(UpdateOrdenServicio request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        var datosAnteriores = new
        {
            orden.Id,
            orden.VehiculoId,
            orden.RecepcionistaId,
            orden.EstadoId,
            orden.CitaId,
            KilometrajeIngreso = orden.KilometrajeIngreso.Value,
            orden.FechaIngreso,
            orden.FechaEstimada,
            orden.FechaEntregaReal,
            Observaciones = orden.Observaciones?.Value
        };

        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        await UserRoleGuard.EnsureRecepcionistaAsync(_uow, request.RecepcionistaId, cancellationToken);

        _ = await _uow.EstadosOrden.GetByIdAsync(request.EstadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        if (request.CitaId.HasValue)
        {
            _ = await _uow.Citas.GetByIdAsync(request.CitaId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Cita no encontrada.");
        }

        orden.Update(
            request.VehiculoId,
            request.RecepcionistaId,
            request.EstadoId,
            request.CitaId,
            KilometrajeIngreso.Create(request.KilometrajeIngreso),
            request.FechaEstimada,
            request.FechaEntregaReal,
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesOrden.Create(request.Observaciones));

        await _uow.OrdenesServicio.UpdateAsync(orden, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _auditoriaService.RegistrarAsync(
            request.RecepcionistaId,
            "OrdenServicio",
            orden.Id,
            "ACTUALIZAR",
            datosAnteriores,
            new
            {
                orden.Id,
                orden.VehiculoId,
                orden.RecepcionistaId,
                orden.EstadoId,
                orden.CitaId,
                KilometrajeIngreso = orden.KilometrajeIngreso.Value,
                orden.FechaIngreso,
                orden.FechaEstimada,
                orden.FechaEntregaReal,
                Observaciones = orden.Observaciones?.Value
            },
            cancellationToken);
    }
}

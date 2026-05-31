using Application.Abstractions;
using Application.Common.Security;
using Domain.Entities;
using Domain.ValueObjects.OrdenServicios;
using FluentValidation;
using MediatR;

namespace Application.OrdenesServicio.UseCase;

public sealed record CreateOrdenServicio(
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly FechaIngreso,
    DateOnly? FechaEstimada,
    string? Observaciones) : IRequest<int>;

public sealed class CreateOrdenServicioValidator : AbstractValidator<CreateOrdenServicio>
{
    public CreateOrdenServicioValidator()
    {
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.RecepcionistaId).GreaterThan(0);
        RuleFor(x => x.EstadoId).GreaterThan(0);
        RuleFor(x => x.KilometrajeIngreso).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateOrdenServicioHandler : IRequestHandler<CreateOrdenServicio, int>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditoriaService _auditoriaService;

    public CreateOrdenServicioHandler(IUnitOfWork uow, IAuditoriaService auditoriaService)
    {
        _uow = uow;
        _auditoriaService = auditoriaService;
    }

    public async Task<int> Handle(CreateOrdenServicio request, CancellationToken cancellationToken)
    {
        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        await UserRoleGuard.EnsureRecepcionistaAsync(_uow, request.RecepcionistaId, cancellationToken);

        _ = await _uow.EstadosOrden.GetByIdAsync(request.EstadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        if (await _uow.OrdenesServicio.HasActiveOrderForVehiculoAsync(request.VehiculoId, cancellationToken))
        {
            throw new InvalidOperationException("El vehiculo ya tiene una orden de servicio activa.");
        }

        if (request.CitaId.HasValue)
        {
            _ = await _uow.Citas.GetByIdAsync(request.CitaId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Cita no encontrada.");
        }

        var orden = new OrdenServicio(
            request.VehiculoId,
            request.RecepcionistaId,
            request.EstadoId,
            request.CitaId,
            KilometrajeIngreso.Create(request.KilometrajeIngreso),
            request.FechaIngreso,
            request.FechaEstimada,
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesOrden.Create(request.Observaciones));

        await _uow.OrdenesServicio.AddAsync(orden, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _auditoriaService.RegistrarAsync(
            request.RecepcionistaId,
            "OrdenServicio",
            orden.Id,
            "CREAR",
            null,
            new
            {
                orden.Id,
                orden.VehiculoId,
                orden.RecepcionistaId,
                orden.EstadoId,
                orden.CitaId,
                KilometrajeIngreso = orden.KilometrajeIngreso.Value,
                orden.FechaIngreso,
                orden.FechaEstimada
            },
            cancellationToken);

        return orden.Id;
    }
}

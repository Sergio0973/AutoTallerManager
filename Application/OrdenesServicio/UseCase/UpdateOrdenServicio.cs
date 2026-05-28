using Application.Abstractions;
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

    public UpdateOrdenServicioHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateOrdenServicio request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.RecepcionistaId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario recepcionista no encontrado.");

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
    }
}

using Application.Abstractions;
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

    public CreateOrdenServicioHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(CreateOrdenServicio request, CancellationToken cancellationToken)
    {
        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.RecepcionistaId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario recepcionista no encontrado.");

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

        return orden.Id;
    }
}

using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.HistorialKilometrajes;
using FluentValidation;
using MediatR;

namespace Application.HistorialesKilometraje.UseCase;

public sealed record CreateHistorialKilometraje(int VehiculoId, int Kilometraje, DateOnly Fecha, string Fuente) : IRequest<int>;

public sealed class CreateHistorialKilometrajeValidator : AbstractValidator<CreateHistorialKilometraje>
{
    public CreateHistorialKilometrajeValidator()
    {
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.Kilometraje).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fuente).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateHistorialKilometrajeHandler : IRequestHandler<CreateHistorialKilometraje, int>
{
    private readonly IUnitOfWork _uow;

    public CreateHistorialKilometrajeHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateHistorialKilometraje request, CancellationToken cancellationToken)
    {
        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        var historial = new HistorialKilometraje(
            request.VehiculoId,
            Kilometraje.Create(request.Kilometraje),
            request.Fecha,
            FuenteRegistro.Create(request.Fuente));

        await _uow.HistorialesKilometraje.AddAsync(historial, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return historial.Id;
    }
}

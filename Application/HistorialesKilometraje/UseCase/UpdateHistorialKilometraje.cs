using Application.Abstractions;
using Domain.ValueObjects.HistorialKilometrajes;
using FluentValidation;
using MediatR;

namespace Application.HistorialesKilometraje.UseCase;

public sealed record UpdateHistorialKilometraje(int Id, int VehiculoId, int Kilometraje, DateOnly Fecha, string Fuente) : IRequest;

public sealed class UpdateHistorialKilometrajeValidator : AbstractValidator<UpdateHistorialKilometraje>
{
    public UpdateHistorialKilometrajeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.Kilometraje).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fuente).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateHistorialKilometrajeHandler : IRequestHandler<UpdateHistorialKilometraje>
{
    private readonly IUnitOfWork _uow;

    public UpdateHistorialKilometrajeHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateHistorialKilometraje request, CancellationToken cancellationToken)
    {
        var historial = await _uow.HistorialesKilometraje.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Historial de kilometraje no encontrado.");

        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        historial.Update(
            request.VehiculoId,
            Kilometraje.Create(request.Kilometraje),
            request.Fecha,
            FuenteRegistro.Create(request.Fuente));

        await _uow.HistorialesKilometraje.UpdateAsync(historial, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

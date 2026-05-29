using Application.Abstractions;
using Domain.ValueObjects.MarcaVehiculos;
using FluentValidation;
using MediatR;

namespace Application.MarcasVehiculo.UseCase;

public sealed record UpdateMarcaVehiculo(int Id, string Nombre) : IRequest;

public sealed class UpdateMarcaVehiculoValidator : AbstractValidator<UpdateMarcaVehiculo>
{
    public UpdateMarcaVehiculoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateMarcaVehiculoHandler : IRequestHandler<UpdateMarcaVehiculo>
{
    private readonly IUnitOfWork _uow;

    public UpdateMarcaVehiculoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateMarcaVehiculo request, CancellationToken cancellationToken)
    {
        var marca = await _uow.MarcasVehiculo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Marca de vehiculo no encontrada.");

        var nombre = NombreMarca.Create(request.Nombre);
        var existing = await _uow.MarcasVehiculo.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe una marca de vehiculo con ese nombre.");
        }

        marca.Update(nombre);
        await _uow.MarcasVehiculo.UpdateAsync(marca, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

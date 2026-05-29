using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.MarcaVehiculos;
using FluentValidation;
using MediatR;

namespace Application.MarcasVehiculo.UseCase;

public sealed record CreateMarcaVehiculo(string Nombre) : IRequest<int>;

public sealed class CreateMarcaVehiculoValidator : AbstractValidator<CreateMarcaVehiculo>
{
    public CreateMarcaVehiculoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateMarcaVehiculoHandler : IRequestHandler<CreateMarcaVehiculo, int>
{
    private readonly IUnitOfWork _uow;

    public CreateMarcaVehiculoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateMarcaVehiculo request, CancellationToken cancellationToken)
    {
        var nombre = NombreMarca.Create(request.Nombre);
        if (await _uow.MarcasVehiculo.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe una marca de vehiculo con ese nombre.");
        }

        var marca = new MarcaVehiculo(nombre);
        await _uow.MarcasVehiculo.AddAsync(marca, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return marca.Id;
    }
}

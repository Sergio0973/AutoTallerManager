using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Vehiculos;
using FluentValidation;
using MediatR;

namespace Application.Vehiculos.UseCase;

public sealed record CreateVehiculo(int ClienteId, int ModeloId, string Vin, short Anio, string Placa, string Color) : IRequest<int>;

public sealed class CreateVehiculoValidator : AbstractValidator<CreateVehiculo>
{
    public CreateVehiculoValidator()
    {
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.ModeloId).GreaterThan(0);
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Anio).InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1));
        RuleFor(x => x.Placa).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateVehiculoHandler : IRequestHandler<CreateVehiculo, int>
{
    private readonly IUnitOfWork _uow;

    public CreateVehiculoHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(CreateVehiculo request, CancellationToken cancellationToken)
    {
        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var vin = Vin.Create(request.Vin);
        if (await _uow.Vehiculos.ExistsVinAsync(vin, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un vehiculo con ese VIN.");
        }

        var placa = Placa.Create(request.Placa);
        if (await _uow.Vehiculos.ExistsPlacaAsync(placa, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un vehiculo con esa placa.");
        }

        var vehiculo = new Vehiculo(
            request.ClienteId,
            request.ModeloId,
            vin,
            AnioVehiculo.Create(request.Anio),
            placa,
            Color.Create(request.Color));

        await _uow.Vehiculos.AddAsync(vehiculo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return vehiculo.Id;
    }
}

using Application.Abstractions;
using Domain.ValueObjects.Vehiculos;
using FluentValidation;
using MediatR;

namespace Application.Vehiculos.UseCase;

public sealed record UpdateVehiculo(int Id, int ClienteId, int ModeloId, string Vin, short Anio, string Placa, string Color) : IRequest;

public sealed class UpdateVehiculoValidator : AbstractValidator<UpdateVehiculo>
{
    public UpdateVehiculoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.ModeloId).GreaterThan(0);
        RuleFor(x => x.Vin).NotEmpty().Length(17);
        RuleFor(x => x.Anio).InclusiveBetween((short)1900, (short)(DateTime.UtcNow.Year + 1));
        RuleFor(x => x.Placa).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateVehiculoHandler : IRequestHandler<UpdateVehiculo>
{
    private readonly IUnitOfWork _uow;

    public UpdateVehiculoHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateVehiculo request, CancellationToken cancellationToken)
    {
        var vehiculo = await _uow.Vehiculos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var vin = Vin.Create(request.Vin);
        var existentePorVin = await _uow.Vehiculos.GetByVinAsync(vin, cancellationToken);
        if (existentePorVin is not null && existentePorVin.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un vehiculo con ese VIN.");
        }

        var placa = Placa.Create(request.Placa);
        var existentePorPlaca = await _uow.Vehiculos.GetByPlacaAsync(placa, cancellationToken);
        if (existentePorPlaca is not null && existentePorPlaca.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un vehiculo con esa placa.");
        }

        vehiculo.Update(
            request.ClienteId,
            request.ModeloId,
            vin,
            AnioVehiculo.Create(request.Anio),
            placa,
            Color.Create(request.Color));

        await _uow.Vehiculos.UpdateAsync(vehiculo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

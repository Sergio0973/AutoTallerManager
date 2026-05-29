using Application.Abstractions;
using Domain.ValueObjects.ModeloVehiculos;
using FluentValidation;
using MediatR;

namespace Application.ModelosVehiculo.UseCase;

public sealed record UpdateModeloVehiculo(int Id, int MarcaId, string Nombre, short AnioDesde, short AnioHasta) : IRequest;

public sealed class UpdateModeloVehiculoValidator : AbstractValidator<UpdateModeloVehiculo>
{
    public UpdateModeloVehiculoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.MarcaId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AnioDesde).GreaterThanOrEqualTo((short)1900);
        RuleFor(x => x.AnioHasta).GreaterThanOrEqualTo((short)1900);
    }
}

public sealed class UpdateModeloVehiculoHandler : IRequestHandler<UpdateModeloVehiculo>
{
    private readonly IUnitOfWork _uow;

    public UpdateModeloVehiculoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateModeloVehiculo request, CancellationToken cancellationToken)
    {
        var modelo = await _uow.ModelosVehiculo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Modelo de vehiculo no encontrado.");

        _ = await _uow.MarcasVehiculo.GetByIdAsync(request.MarcaId, cancellationToken)
            ?? throw new KeyNotFoundException("Marca de vehiculo no encontrada.");

        var nombre = NombreModelo.Create(request.Nombre);
        var existing = await _uow.ModelosVehiculo.GetByMarcaAndNombreAsync(request.MarcaId, nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un modelo con ese nombre para la marca indicada.");
        }

        modelo.Update(request.MarcaId, nombre, RangoAnio.Create(request.AnioDesde, request.AnioHasta));
        await _uow.ModelosVehiculo.UpdateAsync(modelo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

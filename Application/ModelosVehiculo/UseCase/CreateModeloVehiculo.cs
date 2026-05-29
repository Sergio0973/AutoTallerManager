using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ModeloVehiculos;
using FluentValidation;
using MediatR;

namespace Application.ModelosVehiculo.UseCase;

public sealed record CreateModeloVehiculo(int MarcaId, string Nombre, short AnioDesde, short AnioHasta) : IRequest<int>;

public sealed class CreateModeloVehiculoValidator : AbstractValidator<CreateModeloVehiculo>
{
    public CreateModeloVehiculoValidator()
    {
        RuleFor(x => x.MarcaId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AnioDesde).GreaterThanOrEqualTo((short)1900);
        RuleFor(x => x.AnioHasta).GreaterThanOrEqualTo((short)1900);
    }
}

public sealed class CreateModeloVehiculoHandler : IRequestHandler<CreateModeloVehiculo, int>
{
    private readonly IUnitOfWork _uow;

    public CreateModeloVehiculoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateModeloVehiculo request, CancellationToken cancellationToken)
    {
        _ = await _uow.MarcasVehiculo.GetByIdAsync(request.MarcaId, cancellationToken)
            ?? throw new KeyNotFoundException("Marca de vehiculo no encontrada.");

        var nombre = NombreModelo.Create(request.Nombre);
        if (await _uow.ModelosVehiculo.GetByMarcaAndNombreAsync(request.MarcaId, nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un modelo con ese nombre para la marca indicada.");
        }

        var modelo = new ModeloVehiculo(request.MarcaId, nombre, RangoAnio.Create(request.AnioDesde, request.AnioHasta));
        await _uow.ModelosVehiculo.AddAsync(modelo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return modelo.Id;
    }
}

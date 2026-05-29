using Application.Abstractions;
using Domain.ValueObjects.UnidadMedidas;
using FluentValidation;
using MediatR;

namespace Application.UnidadesMedida.UseCase;

public sealed record UpdateUnidadMedida(int Id, string Nombre, string Abreviatura) : IRequest;

public sealed class UpdateUnidadMedidaValidator : AbstractValidator<UpdateUnidadMedida>
{
    public UpdateUnidadMedidaValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Abreviatura).NotEmpty().MaximumLength(10);
    }
}

public sealed class UpdateUnidadMedidaHandler : IRequestHandler<UpdateUnidadMedida>
{
    private readonly IUnitOfWork _uow;

    public UpdateUnidadMedidaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateUnidadMedida request, CancellationToken cancellationToken)
    {
        var unidad = await _uow.UnidadesMedida.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Unidad de medida no encontrada.");

        var nombre = NombreUnidad.Create(request.Nombre);
        var abreviatura = Abreviatura.Create(request.Abreviatura);
        var existingNombre = await _uow.UnidadesMedida.GetByNombreAsync(nombre, cancellationToken);
        if (existingNombre is not null && existingNombre.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe una unidad de medida con ese nombre.");
        }

        var existingAbreviatura = await _uow.UnidadesMedida.GetByAbreviaturaAsync(abreviatura, cancellationToken);
        if (existingAbreviatura is not null && existingAbreviatura.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe una unidad de medida con esa abreviatura.");
        }

        unidad.Update(nombre, abreviatura);
        await _uow.UnidadesMedida.UpdateAsync(unidad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

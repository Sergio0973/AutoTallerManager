using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.UnidadMedidas;
using FluentValidation;
using MediatR;

namespace Application.UnidadesMedida.UseCase;

public sealed record CreateUnidadMedida(string Nombre, string Abreviatura) : IRequest<int>;

public sealed class CreateUnidadMedidaValidator : AbstractValidator<CreateUnidadMedida>
{
    public CreateUnidadMedidaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Abreviatura).NotEmpty().MaximumLength(10);
    }
}

public sealed class CreateUnidadMedidaHandler : IRequestHandler<CreateUnidadMedida, int>
{
    private readonly IUnitOfWork _uow;

    public CreateUnidadMedidaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateUnidadMedida request, CancellationToken cancellationToken)
    {
        var nombre = NombreUnidad.Create(request.Nombre);
        var abreviatura = Abreviatura.Create(request.Abreviatura);

        if (await _uow.UnidadesMedida.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe una unidad de medida con ese nombre.");
        }

        if (await _uow.UnidadesMedida.GetByAbreviaturaAsync(abreviatura, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe una unidad de medida con esa abreviatura.");
        }

        var unidad = new UnidadMedida(nombre, abreviatura);
        await _uow.UnidadesMedida.AddAsync(unidad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return unidad.Id;
    }
}

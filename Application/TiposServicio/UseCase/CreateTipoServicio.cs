using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.TipoServicios;
using FluentValidation;
using MediatR;

namespace Application.TiposServicio.UseCase;

public sealed record CreateTipoServicio(string Nombre, string Descripcion, int DiasEstimados) : IRequest<int>;

public sealed class CreateTipoServicioValidator : AbstractValidator<CreateTipoServicio>
{
    public CreateTipoServicioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DiasEstimados).GreaterThan(0);
    }
}

public sealed class CreateTipoServicioHandler : IRequestHandler<CreateTipoServicio, int>
{
    private readonly IUnitOfWork _uow;

    public CreateTipoServicioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateTipoServicio request, CancellationToken cancellationToken)
    {
        var nombre = NombreTipoServicio.Create(request.Nombre);
        if (await _uow.TiposServicio.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un tipo de servicio con ese nombre.");
        }

        var tipo = new TipoServicio(
            nombre,
            DescripcionTipoServicio.Create(request.Descripcion),
            DiasEstimados.Create(request.DiasEstimados));

        await _uow.TiposServicio.AddAsync(tipo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return tipo.Id;
    }
}

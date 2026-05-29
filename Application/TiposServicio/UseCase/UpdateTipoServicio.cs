using Application.Abstractions;
using Domain.ValueObjects.TipoServicios;
using FluentValidation;
using MediatR;

namespace Application.TiposServicio.UseCase;

public sealed record UpdateTipoServicio(int Id, string Nombre, string Descripcion, int DiasEstimados) : IRequest;

public sealed class UpdateTipoServicioValidator : AbstractValidator<UpdateTipoServicio>
{
    public UpdateTipoServicioValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DiasEstimados).GreaterThan(0);
    }
}

public sealed class UpdateTipoServicioHandler : IRequestHandler<UpdateTipoServicio>
{
    private readonly IUnitOfWork _uow;

    public UpdateTipoServicioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateTipoServicio request, CancellationToken cancellationToken)
    {
        var tipo = await _uow.TiposServicio.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        var nombre = NombreTipoServicio.Create(request.Nombre);
        var existing = await _uow.TiposServicio.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un tipo de servicio con ese nombre.");
        }

        tipo.Update(
            nombre,
            DescripcionTipoServicio.Create(request.Descripcion),
            DiasEstimados.Create(request.DiasEstimados));

        await _uow.TiposServicio.UpdateAsync(tipo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

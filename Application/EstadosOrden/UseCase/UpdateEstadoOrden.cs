using Application.Abstractions;
using Domain.ValueObjects.EstadoOrdenes;
using FluentValidation;
using MediatR;

namespace Application.EstadosOrden.UseCase;

public sealed record UpdateEstadoOrden(int Id, string Nombre, string Descripcion) : IRequest;

public sealed class UpdateEstadoOrdenValidator : AbstractValidator<UpdateEstadoOrden>
{
    public UpdateEstadoOrdenValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class UpdateEstadoOrdenHandler : IRequestHandler<UpdateEstadoOrden>
{
    private readonly IUnitOfWork _uow;

    public UpdateEstadoOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateEstadoOrden request, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosOrden.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        var nombre = NombreEstado.Create(request.Nombre);
        var existing = await _uow.EstadosOrden.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un estado de orden con ese nombre.");
        }

        estado.Update(nombre, DescripcionEstado.Create(request.Descripcion));
        await _uow.EstadosOrden.UpdateAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

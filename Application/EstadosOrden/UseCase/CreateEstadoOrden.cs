using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.EstadoOrdenes;
using FluentValidation;
using MediatR;

namespace Application.EstadosOrden.UseCase;

public sealed record CreateEstadoOrden(string Nombre, string Descripcion) : IRequest<int>;

public sealed class CreateEstadoOrdenValidator : AbstractValidator<CreateEstadoOrden>
{
    public CreateEstadoOrdenValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateEstadoOrdenHandler : IRequestHandler<CreateEstadoOrden, int>
{
    private readonly IUnitOfWork _uow;

    public CreateEstadoOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateEstadoOrden request, CancellationToken cancellationToken)
    {
        var nombre = NombreEstado.Create(request.Nombre);
        if (await _uow.EstadosOrden.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un estado de orden con ese nombre.");
        }

        var estado = new EstadoOrden(nombre, DescripcionEstado.Create(request.Descripcion));
        await _uow.EstadosOrden.AddAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return estado.Id;
    }
}

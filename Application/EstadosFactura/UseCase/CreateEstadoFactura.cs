using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.EstadoFacturas;
using FluentValidation;
using MediatR;

namespace Application.EstadosFactura.UseCase;

public sealed record CreateEstadoFactura(string Nombre) : IRequest<int>;

public sealed class CreateEstadoFacturaValidator : AbstractValidator<CreateEstadoFactura>
{
    public CreateEstadoFacturaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateEstadoFacturaHandler : IRequestHandler<CreateEstadoFactura, int>
{
    private readonly IUnitOfWork _uow;

    public CreateEstadoFacturaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateEstadoFactura request, CancellationToken cancellationToken)
    {
        var nombre = NombreEstadoFactura.Create(request.Nombre);
        if (await _uow.EstadosFactura.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un estado de factura con ese nombre.");
        }

        var estado = new EstadoFactura(nombre);
        await _uow.EstadosFactura.AddAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return estado.Id;
    }
}

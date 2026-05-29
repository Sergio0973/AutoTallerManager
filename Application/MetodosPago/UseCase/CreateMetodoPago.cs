using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.MetodoPagos;
using FluentValidation;
using MediatR;

namespace Application.MetodosPago.UseCase;

public sealed record CreateMetodoPago(string Nombre, string Descripcion) : IRequest<int>;

public sealed class CreateMetodoPagoValidator : AbstractValidator<CreateMetodoPago>
{
    public CreateMetodoPagoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateMetodoPagoHandler : IRequestHandler<CreateMetodoPago, int>
{
    private readonly IUnitOfWork _uow;

    public CreateMetodoPagoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateMetodoPago request, CancellationToken cancellationToken)
    {
        var nombre = NombreMetodoPago.Create(request.Nombre);
        if (await _uow.MetodosPago.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un metodo de pago con ese nombre.");
        }

        var metodo = new MetodoPago(nombre, DescripcionMetodoPago.Create(request.Descripcion));
        await _uow.MetodosPago.AddAsync(metodo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return metodo.Id;
    }
}

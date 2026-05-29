using Application.Abstractions;
using Domain.ValueObjects.MetodoPagos;
using FluentValidation;
using MediatR;

namespace Application.MetodosPago.UseCase;

public sealed record UpdateMetodoPago(int Id, string Nombre, string Descripcion) : IRequest;

public sealed class UpdateMetodoPagoValidator : AbstractValidator<UpdateMetodoPago>
{
    public UpdateMetodoPagoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class UpdateMetodoPagoHandler : IRequestHandler<UpdateMetodoPago>
{
    private readonly IUnitOfWork _uow;

    public UpdateMetodoPagoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateMetodoPago request, CancellationToken cancellationToken)
    {
        var metodo = await _uow.MetodosPago.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Metodo de pago no encontrado.");

        var nombre = NombreMetodoPago.Create(request.Nombre);
        var existing = await _uow.MetodosPago.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un metodo de pago con ese nombre.");
        }

        metodo.Update(nombre, DescripcionMetodoPago.Create(request.Descripcion));
        await _uow.MetodosPago.UpdateAsync(metodo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

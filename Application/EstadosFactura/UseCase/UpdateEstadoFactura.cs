using Application.Abstractions;
using Domain.ValueObjects.EstadoFacturas;
using FluentValidation;
using MediatR;

namespace Application.EstadosFactura.UseCase;

public sealed record UpdateEstadoFactura(int Id, string Nombre) : IRequest;

public sealed class UpdateEstadoFacturaValidator : AbstractValidator<UpdateEstadoFactura>
{
    public UpdateEstadoFacturaValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateEstadoFacturaHandler : IRequestHandler<UpdateEstadoFactura>
{
    private readonly IUnitOfWork _uow;

    public UpdateEstadoFacturaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateEstadoFactura request, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosFactura.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de factura no encontrado.");

        var nombre = NombreEstadoFactura.Create(request.Nombre);
        var existing = await _uow.EstadosFactura.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un estado de factura con ese nombre.");
        }

        estado.Update(nombre);
        await _uow.EstadosFactura.UpdateAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

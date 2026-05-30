using Application.Abstractions;
using Domain.ValueObjects.Facturas;
using FluentValidation;
using MediatR;

namespace Application.Facturas.UseCase;

public sealed record UpdateFactura(
    int Id,
    int EstadoFacturaId,
    decimal ManoDeObra,
    decimal CostoRepuestos,
    decimal Descuento,
    decimal ImpuestoPct,
    decimal Subtotal,
    decimal Total,
    string? Observaciones) : IRequest;

public sealed class UpdateFacturaValidator : AbstractValidator<UpdateFactura>
{
    public UpdateFacturaValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.EstadoFacturaId).GreaterThan(0);
        RuleFor(x => x.ManoDeObra).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostoRepuestos).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Descuento).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImpuestoPct).InclusiveBetween(0, 100);
        RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Total).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateFacturaHandler : IRequestHandler<UpdateFactura>
{
    private readonly IUnitOfWork _uow;

    public UpdateFacturaHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateFactura request, CancellationToken cancellationToken)
    {
        var factura = await _uow.Facturas.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Factura no encontrada.");

        _ = await _uow.EstadosFactura.GetByIdAsync(request.EstadoFacturaId, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de factura no encontrado.");

        factura.Update(
            request.EstadoFacturaId,
            ValoresFactura.Create(
                request.ManoDeObra,
                request.CostoRepuestos,
                request.Descuento,
                request.ImpuestoPct,
                request.Subtotal,
                request.Total),
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesFactura.Create(request.Observaciones));

        await _uow.Facturas.UpdateAsync(factura, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

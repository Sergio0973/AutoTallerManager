using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Facturas;
using FluentValidation;
using MediatR;

namespace Application.Facturas.UseCase;

public sealed record CreateFactura(
    int OrdenId,
    int EstadoFacturaId,
    int UsuarioId,
    decimal ManoDeObra,
    decimal CostoRepuestos,
    decimal Descuento,
    decimal ImpuestoPct,
    decimal Subtotal,
    decimal Total,
    DateOnly FechaEmision,
    string? Observaciones) : IRequest<int>;

public sealed class CreateFacturaValidator : AbstractValidator<CreateFactura>
{
    public CreateFacturaValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.EstadoFacturaId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.ManoDeObra).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostoRepuestos).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Descuento).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImpuestoPct).InclusiveBetween(0, 100);
        RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Total).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateFacturaHandler : IRequestHandler<CreateFactura, int>
{
    private readonly IUnitOfWork _uow;

    public CreateFacturaHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(CreateFactura request, CancellationToken cancellationToken)
    {
        _ = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var existente = await _uow.Facturas.GetByOrdenIdAsync(request.OrdenId, cancellationToken);
        if (existente is not null)
        {
            throw new InvalidOperationException("La orden ya tiene una factura asociada.");
        }

        var factura = new Factura(
            request.OrdenId,
            request.EstadoFacturaId,
            request.UsuarioId,
            ValoresFactura.Create(
                request.ManoDeObra,
                request.CostoRepuestos,
                request.Descuento,
                request.ImpuestoPct,
                request.Subtotal,
                request.Total),
            request.FechaEmision,
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesFactura.Create(request.Observaciones));

        await _uow.Facturas.AddAsync(factura, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return factura.Id;
    }
}

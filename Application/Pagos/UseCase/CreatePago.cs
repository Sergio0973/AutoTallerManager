using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Pagos;
using FluentValidation;
using MediatR;

namespace Application.Pagos.UseCase;

public sealed record CreatePago(
    int FacturaId,
    int MetodoPagoId,
    decimal Monto,
    string? Referencia,
    string Estado) : IRequest<int>;

public sealed class CreatePagoValidator : AbstractValidator<CreatePago>
{
    public CreatePagoValidator()
    {
        RuleFor(x => x.FacturaId).GreaterThan(0);
        RuleFor(x => x.MetodoPagoId).GreaterThan(0);
        RuleFor(x => x.Monto).GreaterThan(0);
        RuleFor(x => x.Referencia).MaximumLength(100);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreatePagoHandler : IRequestHandler<CreatePago, int>
{
    private readonly IUnitOfWork _uow;

    public CreatePagoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreatePago request, CancellationToken cancellationToken)
    {
        _ = await _uow.Facturas.GetByIdAsync(request.FacturaId, cancellationToken)
            ?? throw new KeyNotFoundException("Factura no encontrada.");

        _ = await _uow.MetodosPago.GetByIdAsync(request.MetodoPagoId, cancellationToken)
            ?? throw new KeyNotFoundException("Metodo de pago no encontrado.");

        var pago = new Pago(
            request.FacturaId,
            request.MetodoPagoId,
            MontoPago.Create(request.Monto),
            string.IsNullOrWhiteSpace(request.Referencia) ? null : ReferenciaPago.Create(request.Referencia),
            EstadoPago.Create(request.Estado));

        await _uow.Pagos.AddAsync(pago, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return pago.Id;
    }
}

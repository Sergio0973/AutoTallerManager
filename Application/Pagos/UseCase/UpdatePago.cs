using Application.Abstractions;
using Domain.ValueObjects.Pagos;
using FluentValidation;
using MediatR;

namespace Application.Pagos.UseCase;

public sealed record UpdatePago(int Id, string Estado) : IRequest;

public sealed class UpdatePagoValidator : AbstractValidator<UpdatePago>
{
    public UpdatePagoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdatePagoHandler : IRequestHandler<UpdatePago>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditoriaService _auditoriaService;

    public UpdatePagoHandler(IUnitOfWork uow, IAuditoriaService auditoriaService)
    {
        _uow = uow;
        _auditoriaService = auditoriaService;
    }

    public async Task Handle(UpdatePago request, CancellationToken cancellationToken)
    {
        var pago = await _uow.Pagos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Pago no encontrado.");

        if (string.Equals(pago.Estado.Value, "Confirmado", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("No se puede modificar un pago confirmado.");
        }

        var factura = await _uow.Facturas.GetByIdAsync(pago.FacturaId, cancellationToken)
            ?? throw new KeyNotFoundException("Factura no encontrada.");

        var datosAnteriores = new
        {
            pago.Id,
            pago.FacturaId,
            pago.MetodoPagoId,
            Monto = pago.Monto.Value,
            pago.FechaPago,
            Referencia = pago.Referencia?.Value,
            Estado = pago.Estado.Value
        };

        pago.Update(EstadoPago.Create(request.Estado));
        await _uow.Pagos.UpdateAsync(pago, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _auditoriaService.RegistrarAsync(
            factura.UsuarioId,
            "Pago",
            pago.Id,
            "ACTUALIZAR",
            datosAnteriores,
            new
            {
                pago.Id,
                pago.FacturaId,
                pago.MetodoPagoId,
                Monto = pago.Monto.Value,
                pago.FechaPago,
                Referencia = pago.Referencia?.Value,
                Estado = pago.Estado.Value
            },
            cancellationToken);
    }
}

using Application.Abstractions;
using Domain.ValueObjects.Compras;
using Domain.ValueObjects.DetalleCompras;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleCompras.UseCase;

public sealed record UpdateDetalleCompra(int Id, int Cantidad, decimal PrecioUnitario) : IRequest;

public sealed class UpdateDetalleCompraValidator : AbstractValidator<UpdateDetalleCompra>
{
    public UpdateDetalleCompraValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateDetalleCompraHandler : IRequestHandler<UpdateDetalleCompra>
{
    private readonly IUnitOfWork _uow;

    public UpdateDetalleCompraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateDetalleCompra request, CancellationToken cancellationToken)
    {
        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var detalle = await _uow.DetallesCompra.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Detalle de compra no encontrado.");

            var compra = await _uow.Compras.GetByIdAsync(detalle.CompraId, ct)
                ?? throw new KeyNotFoundException("Compra no encontrada.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(detalle.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            var oldCantidad = detalle.Cantidad.Value;
            var oldSubtotal = detalle.Cantidad.Value * detalle.PrecioUnitario.Value;
            var cantidad = CantidadCompra.Create(request.Cantidad);
            var precio = PrecioUnitarioCompra.Create(request.PrecioUnitario);
            var deltaStock = cantidad.Value - oldCantidad;

            if (deltaStock > 0)
            {
                repuesto.AumentarStock(deltaStock);
            }
            else if (deltaStock < 0)
            {
                repuesto.DisminuirStock(Math.Abs(deltaStock));
            }

            detalle.Update(cantidad, precio);
            var newSubtotal = cantidad.Value * precio.Value;
            compra.Update(
                compra.ProveedorId,
                compra.UsuarioId,
                compra.FechaCompra,
                TotalCompra.Create(compra.Total.Value - oldSubtotal + newSubtotal),
                compra.Estado,
                compra.Observaciones);

            if (deltaStock != 0)
            {
                var log = new Domain.Entities.LogInventario(
                    repuesto.Id,
                    compra.UsuarioId,
                    null,
                    compra.Id,
                    TipoMovimiento.Create("AJUSTE_COMPRA"),
                    deltaStock,
                    repuesto.StockActual,
                    MotivoMovimiento.Create("Ajuste de inventario por detalle de compra."));

                await _uow.LogsInventario.AddAsync(log, ct);
            }

            await _uow.DetallesCompra.UpdateAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.Compras.UpdateAsync(compra, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}

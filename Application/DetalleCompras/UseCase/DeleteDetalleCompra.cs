using Application.Abstractions;
using Domain.ValueObjects.Compras;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleCompras.UseCase;

public sealed record DeleteDetalleCompra(int Id) : IRequest;

public sealed class DeleteDetalleCompraValidator : AbstractValidator<DeleteDetalleCompra>
{
    public DeleteDetalleCompraValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class DeleteDetalleCompraHandler : IRequestHandler<DeleteDetalleCompra>
{
    private readonly IUnitOfWork _uow;

    public DeleteDetalleCompraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteDetalleCompra request, CancellationToken cancellationToken)
    {
        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var detalle = await _uow.DetallesCompra.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Detalle de compra no encontrado.");

            var compra = await _uow.Compras.GetByIdAsync(detalle.CompraId, ct)
                ?? throw new KeyNotFoundException("Compra no encontrada.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(detalle.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            repuesto.DisminuirStock(detalle.Cantidad.Value);
            compra.Update(
                compra.ProveedorId,
                compra.UsuarioId,
                compra.FechaCompra,
                TotalCompra.Create(compra.Total.Value - detalle.Cantidad.Value * detalle.PrecioUnitario.Value),
                compra.Estado,
                compra.Observaciones);

            var log = new Domain.Entities.LogInventario(
                repuesto.Id,
                compra.UsuarioId,
                null,
                compra.Id,
                TipoMovimiento.Create("REVERSO_COMPRA"),
                -detalle.Cantidad.Value,
                repuesto.StockActual,
                MotivoMovimiento.Create("Reverso de inventario por eliminacion de detalle de compra."));

            await _uow.DetallesCompra.RemoveAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.Compras.UpdateAsync(compra, ct);
            await _uow.LogsInventario.AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}

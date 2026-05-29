using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Compras;
using Domain.ValueObjects.DetalleCompras;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleCompras.UseCase;

public sealed record CreateDetalleCompra(int CompraId, int RepuestoId, int Cantidad, decimal PrecioUnitario) : IRequest<int>;

public sealed class CreateDetalleCompraValidator : AbstractValidator<CreateDetalleCompra>
{
    public CreateDetalleCompraValidator()
    {
        RuleFor(x => x.CompraId).GreaterThan(0);
        RuleFor(x => x.RepuestoId).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateDetalleCompraHandler : IRequestHandler<CreateDetalleCompra, int>
{
    private readonly IUnitOfWork _uow;

    public CreateDetalleCompraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateDetalleCompra request, CancellationToken cancellationToken)
    {
        var detalle = default(DetalleCompra);

        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var compra = await _uow.Compras.GetByIdAsync(request.CompraId, ct)
                ?? throw new KeyNotFoundException("Compra no encontrada.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(request.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            if (await _uow.DetallesCompra.GetByCompraAndRepuestoAsync(request.CompraId, request.RepuestoId, ct) is not null)
            {
                throw new InvalidOperationException("La compra ya tiene un detalle para ese repuesto.");
            }

            var cantidad = CantidadCompra.Create(request.Cantidad);
            var precio = PrecioUnitarioCompra.Create(request.PrecioUnitario);
            detalle = new DetalleCompra(request.CompraId, request.RepuestoId, cantidad, precio);

            repuesto.AumentarStock(cantidad.Value);
            compra.Update(
                compra.ProveedorId,
                compra.UsuarioId,
                compra.FechaCompra,
                TotalCompra.Create(compra.Total.Value + cantidad.Value * precio.Value),
                compra.Estado,
                compra.Observaciones);

            var log = new LogInventario(
                repuesto.Id,
                compra.UsuarioId,
                null,
                compra.Id,
                TipoMovimiento.Create("ENTRADA_COMPRA"),
                cantidad.Value,
                repuesto.StockActual,
                MotivoMovimiento.Create("Entrada de inventario por compra."));

            await _uow.DetallesCompra.AddAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.Compras.UpdateAsync(compra, ct);
            await _uow.LogsInventario.AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);

        return detalle!.Id;
    }
}

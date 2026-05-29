using Application.Abstractions;
using Domain.ValueObjects.DetalleOrdenes;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleOrdenes.UseCase;

public sealed record UpdateDetalleOrden(int Id, int UsuarioId, int Cantidad, decimal PrecioSnapshot) : IRequest;

public sealed class UpdateDetalleOrdenValidator : AbstractValidator<UpdateDetalleOrden>
{
    public UpdateDetalleOrdenValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioSnapshot).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateDetalleOrdenHandler : IRequestHandler<UpdateDetalleOrden>
{
    private readonly IUnitOfWork _uow;

    public UpdateDetalleOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateDetalleOrden request, CancellationToken cancellationToken)
    {
        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var detalle = await _uow.DetallesOrden.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Detalle de orden no encontrado.");

            _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, ct)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(detalle.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            var oldCantidad = detalle.Cantidad.Value;
            var cantidad = CantidadOrden.Create(request.Cantidad);
            var delta = cantidad.Value - oldCantidad;

            if (delta > 0)
            {
                repuesto.DisminuirStock(delta);
            }
            else if (delta < 0)
            {
                repuesto.AumentarStock(Math.Abs(delta));
            }

            detalle.Update(cantidad, PrecioSnapshot.Create(request.PrecioSnapshot));

            if (delta != 0)
            {
                var log = new Domain.Entities.LogInventario(
                    repuesto.Id,
                    request.UsuarioId,
                    detalle.OrdenId,
                    null,
                    TipoMovimiento.Create("AJUSTE_ORDEN"),
                    -delta,
                    repuesto.StockActual,
                    MotivoMovimiento.Create("Ajuste de inventario por detalle de orden."));

                await _uow.LogsInventario.AddAsync(log, ct);
            }

            await _uow.DetallesOrden.UpdateAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}

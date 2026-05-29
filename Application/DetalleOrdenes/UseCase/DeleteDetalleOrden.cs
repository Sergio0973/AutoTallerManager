using Application.Abstractions;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleOrdenes.UseCase;

public sealed record DeleteDetalleOrden(int Id, int UsuarioId) : IRequest;

public sealed class DeleteDetalleOrdenValidator : AbstractValidator<DeleteDetalleOrden>
{
    public DeleteDetalleOrdenValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
    }
}

public sealed class DeleteDetalleOrdenHandler : IRequestHandler<DeleteDetalleOrden>
{
    private readonly IUnitOfWork _uow;

    public DeleteDetalleOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteDetalleOrden request, CancellationToken cancellationToken)
    {
        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            var detalle = await _uow.DetallesOrden.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Detalle de orden no encontrado.");

            _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, ct)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(detalle.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            repuesto.AumentarStock(detalle.Cantidad.Value);

            var log = new Domain.Entities.LogInventario(
                repuesto.Id,
                request.UsuarioId,
                detalle.OrdenId,
                null,
                TipoMovimiento.Create("REVERSO_ORDEN"),
                detalle.Cantidad.Value,
                repuesto.StockActual,
                MotivoMovimiento.Create("Reverso de inventario por eliminacion de detalle de orden."));

            await _uow.DetallesOrden.RemoveAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.LogsInventario.AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}

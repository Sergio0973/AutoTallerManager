using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.DetalleOrdenes;
using Domain.ValueObjects.LogInventarios;
using FluentValidation;
using MediatR;

namespace Application.DetalleOrdenes.UseCase;

public sealed record CreateDetalleOrden(int OrdenId, int RepuestoId, int UsuarioId, int Cantidad, decimal PrecioSnapshot) : IRequest<int>;

public sealed class CreateDetalleOrdenValidator : AbstractValidator<CreateDetalleOrden>
{
    public CreateDetalleOrdenValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.RepuestoId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Cantidad).GreaterThan(0);
        RuleFor(x => x.PrecioSnapshot).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateDetalleOrdenHandler : IRequestHandler<CreateDetalleOrden, int>
{
    private readonly IUnitOfWork _uow;

    public CreateDetalleOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateDetalleOrden request, CancellationToken cancellationToken)
    {
        var detalle = default(DetalleOrden);

        await _uow.ExecuteInTransactionAsync(async ct =>
        {
            _ = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, ct)
                ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

            _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, ct)
                ?? throw new KeyNotFoundException("Usuario no encontrado.");

            var repuesto = await _uow.Repuestos.GetByIdAsync(request.RepuestoId, ct)
                ?? throw new KeyNotFoundException("Repuesto no encontrado.");

            if (await _uow.DetallesOrden.GetByOrdenAndRepuestoAsync(request.OrdenId, request.RepuestoId, ct) is not null)
            {
                throw new InvalidOperationException("La orden ya tiene un detalle para ese repuesto.");
            }

            var cantidad = CantidadOrden.Create(request.Cantidad);
            repuesto.DisminuirStock(cantidad.Value);
            detalle = new DetalleOrden(request.OrdenId, request.RepuestoId, cantidad, PrecioSnapshot.Create(request.PrecioSnapshot));

            var log = new LogInventario(
                repuesto.Id,
                request.UsuarioId,
                request.OrdenId,
                null,
                TipoMovimiento.Create("SALIDA_ORDEN"),
                -cantidad.Value,
                repuesto.StockActual,
                MotivoMovimiento.Create("Salida de inventario por orden de servicio."));

            await _uow.DetallesOrden.AddAsync(detalle, ct);
            await _uow.Repuestos.UpdateAsync(repuesto, ct);
            await _uow.LogsInventario.AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }, cancellationToken);

        return detalle!.Id;
    }
}

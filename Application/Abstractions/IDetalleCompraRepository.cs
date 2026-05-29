using Domain.Entities;

namespace Application.Abstractions;

public interface IDetalleCompraRepository
{
    Task<DetalleCompra?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DetalleCompra?> GetByCompraAndRepuestoAsync(int compraId, int repuestoId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleCompra>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DetalleCompra>> GetByCompraIdAsync(int compraId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleCompra>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default);
    Task AddAsync(DetalleCompra detalle, CancellationToken ct = default);
    Task UpdateAsync(DetalleCompra detalle, CancellationToken ct = default);
    Task RemoveAsync(DetalleCompra detalle, CancellationToken ct = default);
}

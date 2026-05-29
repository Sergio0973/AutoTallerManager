using Domain.Entities;

namespace Application.Abstractions;

public interface IDetalleOrdenRepository
{
    Task<DetalleOrden?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DetalleOrden?> GetByOrdenAndRepuestoAsync(int ordenId, int repuestoId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<DetalleOrden>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default);
    Task AddAsync(DetalleOrden detalle, CancellationToken ct = default);
    Task UpdateAsync(DetalleOrden detalle, CancellationToken ct = default);
    Task RemoveAsync(DetalleOrden detalle, CancellationToken ct = default);
}

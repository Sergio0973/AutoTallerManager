using Domain.Entities;

namespace Application.Abstractions;

public interface IRepuestoProveedorRepository
{
    Task<RepuestoProveedor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RepuestoProveedor?> GetByRepuestoAndProveedorAsync(int repuestoId, int proveedorId, CancellationToken ct = default);
    Task<IReadOnlyList<RepuestoProveedor>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RepuestoProveedor>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default);
    Task<IReadOnlyList<RepuestoProveedor>> GetByProveedorIdAsync(int proveedorId, CancellationToken ct = default);
    Task AddAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default);
    Task UpdateAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default);
    Task RemoveAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default);
}

using Domain.Entities;
using Domain.ValueObjects.Proveedores;

namespace Application.Abstractions;

public interface IProveedorRepository
{
    Task<Proveedor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Proveedor?> GetByNitAsync(Nit nit, CancellationToken ct = default);
    Task<Proveedor?> GetByCorreoAsync(CorreoProveedor correo, CancellationToken ct = default);
    Task<IReadOnlyList<Proveedor>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Proveedor>> GetByCiudadIdAsync(int ciudadId, CancellationToken ct = default);
    Task<IReadOnlyList<Proveedor>> GetByActivoAsync(bool activo, CancellationToken ct = default);
    Task AddAsync(Proveedor proveedor, CancellationToken ct = default);
    Task UpdateAsync(Proveedor proveedor, CancellationToken ct = default);
    Task RemoveAsync(Proveedor proveedor, CancellationToken ct = default);
}

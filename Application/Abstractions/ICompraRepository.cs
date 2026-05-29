using Domain.Entities;

namespace Application.Abstractions;

public interface ICompraRepository
{
    Task<Compra?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Compra>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Compra>> GetByProveedorIdAsync(int proveedorId, CancellationToken ct = default);
    Task<IReadOnlyList<Compra>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<Compra>> GetByEstadoAsync(string estado, CancellationToken ct = default);
    Task AddAsync(Compra compra, CancellationToken ct = default);
    Task UpdateAsync(Compra compra, CancellationToken ct = default);
    Task RemoveAsync(Compra compra, CancellationToken ct = default);
}

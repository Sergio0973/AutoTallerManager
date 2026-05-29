using Domain.Entities;

namespace Application.Abstractions;

public interface ILogInventarioRepository
{
    Task<LogInventario?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<LogInventario>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LogInventario>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default);
    Task<IReadOnlyList<LogInventario>> GetByCompraIdAsync(int compraId, CancellationToken ct = default);
    Task<IReadOnlyList<LogInventario>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
    Task AddAsync(LogInventario log, CancellationToken ct = default);
}

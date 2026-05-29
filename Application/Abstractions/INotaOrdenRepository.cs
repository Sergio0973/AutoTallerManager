using Domain.Entities;

namespace Application.Abstractions;

public interface INotaOrdenRepository
{
    Task<NotaOrden?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NotaOrden>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<NotaOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<NotaOrden>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
    Task AddAsync(NotaOrden nota, CancellationToken ct = default);
    Task UpdateAsync(NotaOrden nota, CancellationToken ct = default);
    Task RemoveAsync(NotaOrden nota, CancellationToken ct = default);
}

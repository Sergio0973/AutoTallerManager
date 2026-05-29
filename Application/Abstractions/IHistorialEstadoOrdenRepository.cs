using Domain.Entities;

namespace Application.Abstractions;

public interface IHistorialEstadoOrdenRepository
{
    Task<HistorialEstadoOrden?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<HistorialEstadoOrden>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HistorialEstadoOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<HistorialEstadoOrden>> GetByEstadoIdAsync(int estadoId, CancellationToken ct = default);
    Task<IReadOnlyList<HistorialEstadoOrden>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
    Task AddAsync(HistorialEstadoOrden historial, CancellationToken ct = default);
    Task RemoveAsync(HistorialEstadoOrden historial, CancellationToken ct = default);
}

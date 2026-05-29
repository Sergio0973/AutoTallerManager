using Domain.Entities;
using Domain.ValueObjects.TareaMecanicos;

namespace Application.Abstractions;

public interface ITareaMecanicoRepository
{
    Task<TareaMecanico?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TareaMecanico>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TareaMecanico>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<TareaMecanico>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default);
    Task<IReadOnlyList<TareaMecanico>> GetByEstadoAsync(EstadoTarea estado, CancellationToken ct = default);
    Task AddAsync(TareaMecanico tarea, CancellationToken ct = default);
    Task UpdateAsync(TareaMecanico tarea, CancellationToken ct = default);
    Task RemoveAsync(TareaMecanico tarea, CancellationToken ct = default);
}

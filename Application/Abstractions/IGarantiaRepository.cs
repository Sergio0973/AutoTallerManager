using Domain.Entities;
using Domain.ValueObjects.Garantias;

namespace Application.Abstractions;

public interface IGarantiaRepository
{
    Task<Garantia?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Garantia>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Garantia>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<Garantia>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default);
    Task<IReadOnlyList<Garantia>> GetByEstadoAsync(EstadoGarantia estado, CancellationToken ct = default);
    Task AddAsync(Garantia garantia, CancellationToken ct = default);
    Task UpdateAsync(Garantia garantia, CancellationToken ct = default);
    Task RemoveAsync(Garantia garantia, CancellationToken ct = default);
}

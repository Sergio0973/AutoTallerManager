using Domain.Entities;

namespace Application.Abstractions;

public interface IOrdenMecanicoRepository
{
    Task<OrdenMecanico?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OrdenMecanico?> GetByOrdenAndMecanicoAsync(int ordenId, int mecanicoId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenMecanico>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrdenMecanico>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenMecanico>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default);
    Task AddAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default);
    Task UpdateAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default);
    Task RemoveAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default);
}

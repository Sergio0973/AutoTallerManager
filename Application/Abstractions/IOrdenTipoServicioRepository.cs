using Domain.Entities;

namespace Application.Abstractions;

public interface IOrdenTipoServicioRepository
{
    Task<OrdenTipoServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OrdenTipoServicio?> GetByOrdenAndTipoServicioAsync(int ordenId, int tipoServicioId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenTipoServicio>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrdenTipoServicio>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task AddAsync(OrdenTipoServicio ordenTipoServicio, CancellationToken ct = default);
    Task RemoveAsync(OrdenTipoServicio ordenTipoServicio, CancellationToken ct = default);
}

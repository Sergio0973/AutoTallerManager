using Domain.Entities;

namespace Application.Abstractions;

public interface IOrdenServicioRepository
{
    Task<OrdenServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? estadoId = null, CancellationToken ct = default);
    Task<int> CountAsync(string? search = null, int? estadoId = null, CancellationToken ct = default);

    Task AddAsync(OrdenServicio orden, CancellationToken ct = default);
    Task UpdateAsync(OrdenServicio orden, CancellationToken ct = default);
    Task RemoveAsync(OrdenServicio orden, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

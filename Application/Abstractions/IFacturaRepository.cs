using Domain.Entities;

namespace Application.Abstractions;

public interface IFacturaRepository
{
    Task<Factura?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Factura?> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Factura>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);

    Task AddAsync(Factura factura, CancellationToken ct = default);
    Task UpdateAsync(Factura factura, CancellationToken ct = default);
    Task RemoveAsync(Factura factura, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

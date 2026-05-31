using Domain.Entities;
using Domain.ValueObjects.Clientes;

namespace Application.Abstractions;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Cliente?> GetByDocumentoAsync(Documento documento, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);

    Task AddAsync(Cliente cliente, CancellationToken ct = default);
    Task UpdateAsync(Cliente cliente, CancellationToken ct = default);
    Task RemoveAsync(Cliente cliente, CancellationToken ct = default);
    Task<bool> ExistsDocumentoAsync(Documento documento, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

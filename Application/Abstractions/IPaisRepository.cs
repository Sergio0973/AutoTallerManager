using Domain.Entities;
using Domain.ValueObjects.Paises;

namespace Application.Abstractions;

public interface IPaisRepository
{
    Task<Pais?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Pais?> GetByCodigoAsync(CodigoPais codigo, CancellationToken ct = default);
    Task<IReadOnlyList<Pais>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Pais pais, CancellationToken ct = default);
    Task UpdateAsync(Pais pais, CancellationToken ct = default);
    Task RemoveAsync(Pais pais, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

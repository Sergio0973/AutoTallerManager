using Domain.Entities;
using Domain.ValueObjects.Ciudades;

namespace Application.Abstractions;

public interface ICiudadRepository
{
    Task<Ciudad?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Ciudad?> GetByDepartamentoAndNombreAsync(int departamentoId, NombreCiudad nombre, CancellationToken ct = default);
    Task<IReadOnlyList<Ciudad>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Ciudad>> GetByDepartamentoIdAsync(int departamentoId, CancellationToken ct = default);
    Task AddAsync(Ciudad ciudad, CancellationToken ct = default);
    Task UpdateAsync(Ciudad ciudad, CancellationToken ct = default);
    Task RemoveAsync(Ciudad ciudad, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

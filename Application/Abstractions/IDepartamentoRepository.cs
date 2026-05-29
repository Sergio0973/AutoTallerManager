using Domain.Entities;
using Domain.ValueObjects.Departamentos;

namespace Application.Abstractions;

public interface IDepartamentoRepository
{
    Task<Departamento?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Departamento?> GetByPaisAndNombreAsync(int paisId, NombreDepartamento nombre, CancellationToken ct = default);
    Task<IReadOnlyList<Departamento>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Departamento>> GetByPaisIdAsync(int paisId, CancellationToken ct = default);
    Task AddAsync(Departamento departamento, CancellationToken ct = default);
    Task UpdateAsync(Departamento departamento, CancellationToken ct = default);
    Task RemoveAsync(Departamento departamento, CancellationToken ct = default);
}

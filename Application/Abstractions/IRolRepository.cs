using Domain.Entities;
using Domain.ValueObjects.Roles;

namespace Application.Abstractions;

public interface IRolRepository
{
    Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Rol?> GetByNombreAsync(NombreRol nombre, CancellationToken ct = default);
    Task<IReadOnlyList<Rol>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Rol rol, CancellationToken ct = default);
    Task UpdateAsync(Rol rol, CancellationToken ct = default);
    Task RemoveAsync(Rol rol, CancellationToken ct = default);
}

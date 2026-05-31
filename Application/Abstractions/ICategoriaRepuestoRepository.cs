using Domain.Entities;
using Domain.ValueObjects.CategoriaRepuestos;

namespace Application.Abstractions;

public interface ICategoriaRepuestoRepository
{
    Task<CategoriaRepuesto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoriaRepuesto?> GetByNombreAsync(NombreCategoria nombre, CancellationToken ct = default);
    Task<IReadOnlyList<CategoriaRepuesto>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(CategoriaRepuesto categoria, CancellationToken ct = default);
    Task UpdateAsync(CategoriaRepuesto categoria, CancellationToken ct = default);
    Task RemoveAsync(CategoriaRepuesto categoria, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

using Domain.Entities;
using Domain.ValueObjects.UnidadMedidas;

namespace Application.Abstractions;

public interface IUnidadMedidaRepository
{
    Task<UnidadMedida?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<UnidadMedida?> GetByNombreAsync(NombreUnidad nombre, CancellationToken ct = default);
    Task<UnidadMedida?> GetByAbreviaturaAsync(Abreviatura abreviatura, CancellationToken ct = default);
    Task<IReadOnlyList<UnidadMedida>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(UnidadMedida unidad, CancellationToken ct = default);
    Task UpdateAsync(UnidadMedida unidad, CancellationToken ct = default);
    Task RemoveAsync(UnidadMedida unidad, CancellationToken ct = default);
}

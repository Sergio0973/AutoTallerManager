using Domain.Entities;
using Domain.ValueObjects.TipoServicios;

namespace Application.Abstractions;

public interface ITipoServicioRepository
{
    Task<TipoServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TipoServicio?> GetByNombreAsync(NombreTipoServicio nombre, CancellationToken ct = default);
    Task<IReadOnlyList<TipoServicio>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(TipoServicio tipoServicio, CancellationToken ct = default);
    Task UpdateAsync(TipoServicio tipoServicio, CancellationToken ct = default);
    Task RemoveAsync(TipoServicio tipoServicio, CancellationToken ct = default);
}

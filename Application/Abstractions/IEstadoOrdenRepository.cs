using Domain.Entities;
using Domain.ValueObjects.EstadoOrdenes;

namespace Application.Abstractions;

public interface IEstadoOrdenRepository
{
    Task<EstadoOrden?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EstadoOrden?> GetByNombreAsync(NombreEstado nombre, CancellationToken ct = default);
    Task<IReadOnlyList<EstadoOrden>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(EstadoOrden estado, CancellationToken ct = default);
    Task UpdateAsync(EstadoOrden estado, CancellationToken ct = default);
    Task RemoveAsync(EstadoOrden estado, CancellationToken ct = default);
}

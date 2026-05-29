using Domain.Entities;
using Domain.ValueObjects.EstadoFacturas;

namespace Application.Abstractions;

public interface IEstadoFacturaRepository
{
    Task<EstadoFactura?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EstadoFactura?> GetByNombreAsync(NombreEstadoFactura nombre, CancellationToken ct = default);
    Task<IReadOnlyList<EstadoFactura>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(EstadoFactura estado, CancellationToken ct = default);
    Task UpdateAsync(EstadoFactura estado, CancellationToken ct = default);
    Task RemoveAsync(EstadoFactura estado, CancellationToken ct = default);
}

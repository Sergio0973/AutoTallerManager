using Domain.Entities;
using Domain.ValueObjects.MetodoPagos;

namespace Application.Abstractions;

public interface IMetodoPagoRepository
{
    Task<MetodoPago?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MetodoPago?> GetByNombreAsync(NombreMetodoPago nombre, CancellationToken ct = default);
    Task<IReadOnlyList<MetodoPago>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(MetodoPago metodo, CancellationToken ct = default);
    Task UpdateAsync(MetodoPago metodo, CancellationToken ct = default);
    Task RemoveAsync(MetodoPago metodo, CancellationToken ct = default);
}

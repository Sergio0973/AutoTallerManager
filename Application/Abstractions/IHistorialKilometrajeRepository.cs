using Domain.Entities;

namespace Application.Abstractions;

public interface IHistorialKilometrajeRepository
{
    Task<HistorialKilometraje?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<HistorialKilometraje>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HistorialKilometraje>> GetByVehiculoIdAsync(int vehiculoId, CancellationToken ct = default);
    Task AddAsync(HistorialKilometraje historial, CancellationToken ct = default);
    Task UpdateAsync(HistorialKilometraje historial, CancellationToken ct = default);
    Task RemoveAsync(HistorialKilometraje historial, CancellationToken ct = default);
}

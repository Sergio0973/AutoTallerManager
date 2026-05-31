using Domain.Entities;
using Domain.ValueObjects.Vehiculos;

namespace Application.Abstractions;

public interface IVehiculoRepository
{
    Task<Vehiculo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Vehiculo?> GetByVinAsync(Vin vin, CancellationToken ct = default);
    Task<Vehiculo?> GetByPlacaAsync(Placa placa, CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Vehiculo>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);

    Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default);
    Task UpdateAsync(Vehiculo vehiculo, CancellationToken ct = default);
    Task RemoveAsync(Vehiculo vehiculo, CancellationToken ct = default);
    Task<bool> ExistsVinAsync(Vin vin, CancellationToken ct = default);
    Task<bool> ExistsPlacaAsync(Placa placa, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

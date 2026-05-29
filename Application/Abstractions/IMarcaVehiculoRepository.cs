using Domain.Entities;
using Domain.ValueObjects.MarcaVehiculos;

namespace Application.Abstractions;

public interface IMarcaVehiculoRepository
{
    Task<MarcaVehiculo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<MarcaVehiculo?> GetByNombreAsync(NombreMarca nombre, CancellationToken ct = default);
    Task<IReadOnlyList<MarcaVehiculo>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(MarcaVehiculo marca, CancellationToken ct = default);
    Task UpdateAsync(MarcaVehiculo marca, CancellationToken ct = default);
    Task RemoveAsync(MarcaVehiculo marca, CancellationToken ct = default);
}

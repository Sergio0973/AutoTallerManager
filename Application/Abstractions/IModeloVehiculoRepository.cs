using Domain.Entities;
using Domain.ValueObjects.ModeloVehiculos;

namespace Application.Abstractions;

public interface IModeloVehiculoRepository
{
    Task<ModeloVehiculo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ModeloVehiculo?> GetByMarcaAndNombreAsync(int marcaId, NombreModelo nombre, CancellationToken ct = default);
    Task<IReadOnlyList<ModeloVehiculo>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ModeloVehiculo>> GetByMarcaIdAsync(int marcaId, CancellationToken ct = default);
    Task AddAsync(ModeloVehiculo modelo, CancellationToken ct = default);
    Task UpdateAsync(ModeloVehiculo modelo, CancellationToken ct = default);
    Task RemoveAsync(ModeloVehiculo modelo, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

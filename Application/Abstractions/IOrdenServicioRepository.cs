using Domain.Entities;

namespace Application.Abstractions;

public interface IOrdenServicioRepository
{
    Task<OrdenServicio?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrdenServicio>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? estadoId = null,
        int? vehiculoId = null,
        int? recepcionistaId = null,
        DateOnly? fechaIngresoDesde = null,
        DateOnly? fechaIngresoHasta = null,
        CancellationToken ct = default);

    Task<int> CountAsync(
        string? search = null,
        int? estadoId = null,
        int? vehiculoId = null,
        int? recepcionistaId = null,
        DateOnly? fechaIngresoDesde = null,
        DateOnly? fechaIngresoHasta = null,
        CancellationToken ct = default);

    Task AddAsync(OrdenServicio orden, CancellationToken ct = default);
    Task UpdateAsync(OrdenServicio orden, CancellationToken ct = default);
    Task RemoveAsync(OrdenServicio orden, CancellationToken ct = default);
    Task<bool> HasActiveOrderForVehiculoAsync(int vehiculoId, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

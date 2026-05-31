using Domain.Entities;
using Domain.ValueObjects.Repuestos;

namespace Application.Abstractions;

public interface IRepuestoRepository
{
    Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Repuesto?> GetByCodigoAsync(CodigoRepuesto codigo, CancellationToken ct = default);
    Task<IReadOnlyList<Repuesto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Repuesto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? categoriaId = null,
        int? stockMinimo = null,
        bool? soloBajoStock = null,
        CancellationToken ct = default);

    Task<int> CountAsync(
        string? search = null,
        int? categoriaId = null,
        int? stockMinimo = null,
        bool? soloBajoStock = null,
        CancellationToken ct = default);

    Task AddAsync(Repuesto repuesto, CancellationToken ct = default);
    Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default);
    Task RemoveAsync(Repuesto repuesto, CancellationToken ct = default);
    Task<bool> ExistsCodigoAsync(CodigoRepuesto codigo, CancellationToken ct = default);
    Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default);
}

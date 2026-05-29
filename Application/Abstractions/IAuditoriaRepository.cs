using Domain.Entities;
using Domain.ValueObjects.Auditorias;

namespace Application.Abstractions;

public interface IAuditoriaRepository
{
    Task<Auditoria?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetByEntidadAsync(EntidadAuditada entidad, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetByTipoAccionAsync(TipoAccion tipoAccion, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetByFechaRangeAsync(DateTime desde, DateTime hasta, CancellationToken ct = default);
    Task AddAsync(Auditoria auditoria, CancellationToken ct = default);
}

using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Auditorias;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AutoTallerDbContext _context;

    public AuditoriaRepository(AutoTallerDbContext context) => _context = context;

    public Task<Auditoria?> GetByIdAsync(long id, CancellationToken ct = default) =>
        _context.Auditorias.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Auditoria>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Auditorias.OrderByDescending(a => a.Fecha).ToListAsync(ct);

    public async Task<IReadOnlyList<Auditoria>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default) =>
        await _context.Auditorias
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Auditoria>> GetByEntidadAsync(EntidadAuditada entidad, CancellationToken ct = default) =>
        await _context.Auditorias
            .Where(a => a.Entidad == entidad)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Auditoria>> GetByTipoAccionAsync(TipoAccion tipoAccion, CancellationToken ct = default) =>
        await _context.Auditorias
            .Where(a => a.TipoAccion == tipoAccion)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Auditoria>> GetByFechaRangeAsync(DateTime desde, DateTime hasta, CancellationToken ct = default) =>
        await _context.Auditorias
            .Where(a => a.Fecha >= desde && a.Fecha <= hasta)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync(ct);

    public async Task AddAsync(Auditoria auditoria, CancellationToken ct = default) =>
        await _context.Auditorias.AddAsync(auditoria, ct);
}

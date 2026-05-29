using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class LogInventarioRepository : ILogInventarioRepository
{
    private readonly AutoTallerDbContext _context;

    public LogInventarioRepository(AutoTallerDbContext context) => _context = context;

    public Task<LogInventario?> GetByIdAsync(long id, CancellationToken ct = default) =>
        _context.LogsInventario.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IReadOnlyList<LogInventario>> GetAllAsync(CancellationToken ct = default) =>
        await _context.LogsInventario.OrderByDescending(l => l.Fecha).ThenByDescending(l => l.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<LogInventario>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default) =>
        await _context.LogsInventario.Where(l => l.RepuestoId == repuestoId).OrderByDescending(l => l.Fecha).ToListAsync(ct);

    public async Task<IReadOnlyList<LogInventario>> GetByCompraIdAsync(int compraId, CancellationToken ct = default) =>
        await _context.LogsInventario.Where(l => l.CompraId == compraId).OrderByDescending(l => l.Fecha).ToListAsync(ct);

    public async Task<IReadOnlyList<LogInventario>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default) =>
        await _context.LogsInventario.Where(l => l.UsuarioId == usuarioId).OrderByDescending(l => l.Fecha).ToListAsync(ct);

    public async Task AddAsync(LogInventario log, CancellationToken ct = default) =>
        await _context.LogsInventario.AddAsync(log, ct);
}

using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class HistorialEstadoOrdenRepository : IHistorialEstadoOrdenRepository
{
    private readonly AutoTallerDbContext _context;

    public HistorialEstadoOrdenRepository(AutoTallerDbContext context) => _context = context;

    public Task<HistorialEstadoOrden?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.HistorialEstadosOrden.FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IReadOnlyList<HistorialEstadoOrden>> GetAllAsync(CancellationToken ct = default) =>
        await _context.HistorialEstadosOrden
            .OrderByDescending(h => h.FechaCambio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<HistorialEstadoOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.HistorialEstadosOrden
            .Where(h => h.OrdenId == ordenId)
            .OrderByDescending(h => h.FechaCambio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<HistorialEstadoOrden>> GetByEstadoIdAsync(int estadoId, CancellationToken ct = default) =>
        await _context.HistorialEstadosOrden
            .Where(h => h.EstadoId == estadoId)
            .OrderByDescending(h => h.FechaCambio)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<HistorialEstadoOrden>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default) =>
        await _context.HistorialEstadosOrden
            .Where(h => h.UsuarioId == usuarioId)
            .OrderByDescending(h => h.FechaCambio)
            .ToListAsync(ct);

    public async Task AddAsync(HistorialEstadoOrden historial, CancellationToken ct = default) =>
        await _context.HistorialEstadosOrden.AddAsync(historial, ct);

    public Task RemoveAsync(HistorialEstadoOrden historial, CancellationToken ct = default)
    {
        _context.HistorialEstadosOrden.Remove(historial);
        return Task.CompletedTask;
    }
}

using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class OrdenServicioRepository : IOrdenServicioRepository
{
    private readonly AutoTallerDbContext _context;

    public OrdenServicioRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<OrdenServicio?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.OrdenesServicio
            .Include(o => o.Vehiculo)
            .Include(o => o.Detalles)
            .Include(o => o.Mecanicos)
            .Include(o => o.Tareas)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IReadOnlyList<OrdenServicio>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.OrdenesServicio.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OrdenServicio>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? estadoId = null, CancellationToken ct = default)
    {
        var query = _context.OrdenesServicio.AsQueryable();

        if (estadoId.HasValue)
        {
            query = query.Where(o => o.EstadoId == estadoId.Value);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, int? estadoId = null, CancellationToken ct = default)
    {
        var query = _context.OrdenesServicio.AsQueryable();

        if (estadoId.HasValue)
        {
            query = query.Where(o => o.EstadoId == estadoId.Value);
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(OrdenServicio orden, CancellationToken ct = default)
    {
        await _context.OrdenesServicio.AddAsync(orden, ct);
    }

    public Task UpdateAsync(OrdenServicio orden, CancellationToken ct = default)
    {
        _context.OrdenesServicio.Update(orden);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(OrdenServicio orden, CancellationToken ct = default)
    {
        _context.OrdenesServicio.Remove(orden);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.OrdenesMecanicos.AnyAsync(o => o.OrdenId == id, ct)
            || await _context.OrdenesTiposServicio.AnyAsync(o => o.OrdenId == id, ct)
            || await _context.DetallesOrden.AnyAsync(d => d.OrdenId == id, ct)
            || await _context.TareasMecanicos.AnyAsync(t => t.OrdenId == id, ct)
            || await _context.NotasOrden.AnyAsync(n => n.OrdenId == id, ct)
            || await _context.HistorialEstadosOrden.AnyAsync(h => h.OrdenId == id, ct)
            || await _context.Facturas.AnyAsync(f => f.OrdenId == id, ct)
            || await _context.Garantias.AnyAsync(g => g.OrdenId == id, ct)
            || await _context.LogsInventario.AnyAsync(l => l.OrdenId == id, ct);
    }
}

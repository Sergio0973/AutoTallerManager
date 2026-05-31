using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class FacturaRepository : IFacturaRepository
{
    private readonly AutoTallerDbContext _context;

    public FacturaRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Factura?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Facturas.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<Factura?> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default)
    {
        return await _context.Facturas.FirstOrDefaultAsync(f => f.OrdenId == ordenId, ct);
    }

    public async Task<IReadOnlyList<Factura>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Facturas.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Factura>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = _context.Facturas.AsQueryable();

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = _context.Facturas.AsQueryable();
        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Factura factura, CancellationToken ct = default)
    {
        await _context.Facturas.AddAsync(factura, ct);
    }

    public Task UpdateAsync(Factura factura, CancellationToken ct = default)
    {
        _context.Facturas.Update(factura);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Factura factura, CancellationToken ct = default)
    {
        _context.Facturas.Remove(factura);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Pagos.AnyAsync(p => p.FacturaId == id, ct);
    }
}

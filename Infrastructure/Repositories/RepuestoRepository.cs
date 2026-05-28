using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Repuestos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class RepuestoRepository : IRepuestoRepository
{
    private readonly AutoTallerDbContext _context;

    public RepuestoRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Repuesto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Repuestos.FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<Repuesto?> GetByCodigoAsync(CodigoRepuesto codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos.FirstOrDefaultAsync(r => r.Codigo == codigo, ct);
    }

    public async Task<IReadOnlyList<Repuesto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Repuestos.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Repuesto>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? categoriaId = null, CancellationToken ct = default)
    {
        var query = _context.Repuestos.AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(r => r.CategoriaId == categoriaId.Value);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, int? categoriaId = null, CancellationToken ct = default)
    {
        var query = _context.Repuestos.AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(r => r.CategoriaId == categoriaId.Value);
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        await _context.Repuestos.AddAsync(repuesto, ct);
    }

    public Task UpdateAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        _context.Repuestos.Update(repuesto);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Repuesto repuesto, CancellationToken ct = default)
    {
        _context.Repuestos.Remove(repuesto);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsCodigoAsync(CodigoRepuesto codigo, CancellationToken ct = default)
    {
        return await _context.Repuestos.AnyAsync(r => r.Codigo == codigo, ct);
    }
}

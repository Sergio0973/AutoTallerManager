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

    public async Task<IReadOnlyList<Repuesto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? categoriaId = null,
        int? stockMinimo = null,
        bool? soloBajoStock = null,
        CancellationToken ct = default)
    {
        var query = CreateSearchQuery(search);
        query = ApplyFilters(query, categoriaId, stockMinimo, soloBajoStock);

        return await query
            .OrderBy(r => r.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        string? search = null,
        int? categoriaId = null,
        int? stockMinimo = null,
        bool? soloBajoStock = null,
        CancellationToken ct = default)
    {
        var query = CreateSearchQuery(search);
        query = ApplyFilters(query, categoriaId, stockMinimo, soloBajoStock);

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

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.DetallesOrden.AnyAsync(d => d.RepuestoId == id, ct)
            || await _context.DetallesCompra.AnyAsync(d => d.RepuestoId == id, ct)
            || await _context.RepuestosProveedor.AnyAsync(r => r.RepuestoId == id, ct)
            || await _context.LogsInventario.AnyAsync(l => l.RepuestoId == id, ct);
    }

    private IQueryable<Repuesto> CreateSearchQuery(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return _context.Repuestos.AsQueryable();
        }

        var pattern = $"%{search.Trim().ToUpperInvariant()}%";
        return _context.Repuestos.FromSqlInterpolated(
            $"SELECT * FROM \"Repuestos\" WHERE upper(\"Codigo\") LIKE {pattern} OR upper(\"Descripcion\") LIKE {pattern}");
    }

    private static IQueryable<Repuesto> ApplyFilters(
        IQueryable<Repuesto> query,
        int? categoriaId,
        int? stockMinimo,
        bool? soloBajoStock)
    {
        if (categoriaId.HasValue)
        {
            query = query.Where(r => r.CategoriaId == categoriaId.Value);
        }

        if (stockMinimo.HasValue)
        {
            query = query.Where(r => r.StockActual <= stockMinimo.Value);
        }

        if (soloBajoStock == true)
        {
            query = query.Where(r => r.StockActual <= r.StockMinimo);
        }

        return query;
    }
}

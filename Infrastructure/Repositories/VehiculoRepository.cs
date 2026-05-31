using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Vehiculos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class VehiculoRepository : IVehiculoRepository
{
    private readonly AutoTallerDbContext _context;

    public VehiculoRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Vehiculo?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vehiculo?> GetByVinAsync(Vin vin, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .FirstOrDefaultAsync(v => v.Vin == vin, ct);
    }

    public async Task<Vehiculo?> GetByPlacaAsync(Placa placa, CancellationToken ct = default)
    {
        return await _context.Vehiculos
            .FirstOrDefaultAsync(v => v.Placa == placa, ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Vehiculos.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehiculo>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? clienteId = null,
        string? vin = null,
        string? placa = null,
        CancellationToken ct = default)
    {
        var query = ApplyFilters(_context.Vehiculos.AsQueryable(), search, clienteId, vin, placa);

        return await query
            .OrderBy(v => v.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        string? search = null,
        int? clienteId = null,
        string? vin = null,
        string? placa = null,
        CancellationToken ct = default)
    {
        var query = ApplyFilters(_context.Vehiculos.AsQueryable(), search, clienteId, vin, placa);
        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        await _context.Vehiculos.AddAsync(vehiculo, ct);
    }

    public Task UpdateAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Update(vehiculo);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Vehiculo vehiculo, CancellationToken ct = default)
    {
        _context.Vehiculos.Remove(vehiculo);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsVinAsync(Vin vin, CancellationToken ct = default)
    {
        return await _context.Vehiculos.AnyAsync(v => v.Vin == vin, ct);
    }

    public async Task<bool> ExistsPlacaAsync(Placa placa, CancellationToken ct = default)
    {
        return await _context.Vehiculos.AnyAsync(v => v.Placa == placa, ct);
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Citas.AnyAsync(c => c.VehiculoId == id, ct)
            || await _context.OrdenesServicio.AnyAsync(o => o.VehiculoId == id, ct)
            || await _context.HistorialesKilometraje.AnyAsync(h => h.VehiculoId == id, ct);
    }

    private static IQueryable<Vehiculo> ApplyFilters(
        IQueryable<Vehiculo> query,
        string? search,
        int? clienteId,
        string? vin,
        string? placa)
    {
        if (clienteId.HasValue)
        {
            query = query.Where(v => v.ClienteId == clienteId.Value);
        }

        if (!string.IsNullOrWhiteSpace(vin))
        {
            var normalizedVin = vin.Trim().ToUpperInvariant();
            query = query.Where(v => v.Vin.Value.Contains(normalizedVin));
        }

        if (!string.IsNullOrWhiteSpace(placa))
        {
            var normalizedPlaca = placa.Trim().ToUpperInvariant();
            query = query.Where(v => v.Placa.Value.Contains(normalizedPlaca));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToUpperInvariant();
            query = query.Where(v =>
                v.Vin.Value.Contains(term) ||
                v.Placa.Value.Contains(term) ||
                v.Color.Value.ToUpper().Contains(term));
        }

        return query;
    }
}

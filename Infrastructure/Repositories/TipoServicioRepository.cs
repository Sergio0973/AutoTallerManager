using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.TipoServicios;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TipoServicioRepository : ITipoServicioRepository
{
    private readonly AutoTallerDbContext _context;

    public TipoServicioRepository(AutoTallerDbContext context) => _context = context;

    public Task<TipoServicio?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.TiposServicio.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<TipoServicio?> GetByNombreAsync(NombreTipoServicio nombre, CancellationToken ct = default) =>
        _context.TiposServicio.FirstOrDefaultAsync(t => t.Nombre == nombre, ct);

    public async Task<IReadOnlyList<TipoServicio>> GetAllAsync(CancellationToken ct = default) =>
        await _context.TiposServicio.OrderBy(t => t.Id).ToListAsync(ct);

    public async Task AddAsync(TipoServicio tipoServicio, CancellationToken ct = default) =>
        await _context.TiposServicio.AddAsync(tipoServicio, ct);

    public Task UpdateAsync(TipoServicio tipoServicio, CancellationToken ct = default)
    {
        _context.TiposServicio.Update(tipoServicio);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TipoServicio tipoServicio, CancellationToken ct = default)
    {
        _context.TiposServicio.Remove(tipoServicio);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Citas.AnyAsync(c => c.TipoServicioId == id, ct)
            || await _context.OrdenesTiposServicio.AnyAsync(o => o.TipoServicioId == id, ct)
            || await _context.TareasMecanicos.AnyAsync(t => t.TipoServicioId == id, ct)
            || await _context.Garantias.AnyAsync(g => g.TipoServicioId == id, ct);
    }
}

using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class OrdenTipoServicioRepository : IOrdenTipoServicioRepository
{
    private readonly AutoTallerDbContext _context;

    public OrdenTipoServicioRepository(AutoTallerDbContext context) => _context = context;

    public Task<OrdenTipoServicio?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.OrdenesTiposServicio.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<OrdenTipoServicio?> GetByOrdenAndTipoServicioAsync(int ordenId, int tipoServicioId, CancellationToken ct = default) =>
        _context.OrdenesTiposServicio.FirstOrDefaultAsync(x => x.OrdenId == ordenId && x.TipoServicioId == tipoServicioId, ct);

    public async Task<IReadOnlyList<OrdenTipoServicio>> GetAllAsync(CancellationToken ct = default) =>
        await _context.OrdenesTiposServicio.OrderBy(x => x.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<OrdenTipoServicio>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.OrdenesTiposServicio.Where(x => x.OrdenId == ordenId).OrderBy(x => x.Id).ToListAsync(ct);

    public async Task AddAsync(OrdenTipoServicio ordenTipoServicio, CancellationToken ct = default) =>
        await _context.OrdenesTiposServicio.AddAsync(ordenTipoServicio, ct);

    public Task RemoveAsync(OrdenTipoServicio ordenTipoServicio, CancellationToken ct = default)
    {
        _context.OrdenesTiposServicio.Remove(ordenTipoServicio);
        return Task.CompletedTask;
    }
}

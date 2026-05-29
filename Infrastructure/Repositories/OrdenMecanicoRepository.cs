using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class OrdenMecanicoRepository : IOrdenMecanicoRepository
{
    private readonly AutoTallerDbContext _context;

    public OrdenMecanicoRepository(AutoTallerDbContext context) => _context = context;

    public Task<OrdenMecanico?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.OrdenesMecanicos.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<OrdenMecanico?> GetByOrdenAndMecanicoAsync(int ordenId, int mecanicoId, CancellationToken ct = default) =>
        _context.OrdenesMecanicos.FirstOrDefaultAsync(x => x.OrdenId == ordenId && x.MecanicoId == mecanicoId, ct);

    public async Task<IReadOnlyList<OrdenMecanico>> GetAllAsync(CancellationToken ct = default) =>
        await _context.OrdenesMecanicos.OrderBy(x => x.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<OrdenMecanico>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.OrdenesMecanicos.Where(x => x.OrdenId == ordenId).OrderBy(x => x.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<OrdenMecanico>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default) =>
        await _context.OrdenesMecanicos.Where(x => x.MecanicoId == mecanicoId).OrderBy(x => x.Id).ToListAsync(ct);

    public async Task AddAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default) =>
        await _context.OrdenesMecanicos.AddAsync(ordenMecanico, ct);

    public Task UpdateAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default)
    {
        _context.OrdenesMecanicos.Update(ordenMecanico);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(OrdenMecanico ordenMecanico, CancellationToken ct = default)
    {
        _context.OrdenesMecanicos.Remove(ordenMecanico);
        return Task.CompletedTask;
    }
}

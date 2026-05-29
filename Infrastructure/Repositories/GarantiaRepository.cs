using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Garantias;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class GarantiaRepository : IGarantiaRepository
{
    private readonly AutoTallerDbContext _context;

    public GarantiaRepository(AutoTallerDbContext context) => _context = context;

    public Task<Garantia?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Garantias.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<Garantia>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Garantias.OrderBy(g => g.FechaVencimiento).ThenBy(g => g.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Garantia>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.Garantias.Where(g => g.OrdenId == ordenId).OrderBy(g => g.FechaVencimiento).ToListAsync(ct);

    public async Task<IReadOnlyList<Garantia>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default) =>
        await _context.Garantias.Where(g => g.MecanicoId == mecanicoId).OrderBy(g => g.FechaVencimiento).ToListAsync(ct);

    public async Task<IReadOnlyList<Garantia>> GetByEstadoAsync(EstadoGarantia estado, CancellationToken ct = default) =>
        await _context.Garantias.Where(g => g.Estado == estado).OrderBy(g => g.FechaVencimiento).ToListAsync(ct);

    public async Task AddAsync(Garantia garantia, CancellationToken ct = default) =>
        await _context.Garantias.AddAsync(garantia, ct);

    public Task UpdateAsync(Garantia garantia, CancellationToken ct = default)
    {
        _context.Garantias.Update(garantia);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Garantia garantia, CancellationToken ct = default)
    {
        _context.Garantias.Remove(garantia);
        return Task.CompletedTask;
    }
}

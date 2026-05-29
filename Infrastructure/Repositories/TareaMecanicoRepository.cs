using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.TareaMecanicos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class TareaMecanicoRepository : ITareaMecanicoRepository
{
    private readonly AutoTallerDbContext _context;

    public TareaMecanicoRepository(AutoTallerDbContext context) => _context = context;

    public Task<TareaMecanico?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.TareasMecanicos.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<TareaMecanico>> GetAllAsync(CancellationToken ct = default) =>
        await _context.TareasMecanicos.OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<TareaMecanico>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.TareasMecanicos.Where(t => t.OrdenId == ordenId).OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<TareaMecanico>> GetByMecanicoIdAsync(int mecanicoId, CancellationToken ct = default) =>
        await _context.TareasMecanicos.Where(t => t.MecanicoId == mecanicoId).OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<TareaMecanico>> GetByEstadoAsync(EstadoTarea estado, CancellationToken ct = default) =>
        await _context.TareasMecanicos.Where(t => t.Estado == estado).OrderBy(t => t.Id).ToListAsync(ct);

    public async Task AddAsync(TareaMecanico tarea, CancellationToken ct = default) =>
        await _context.TareasMecanicos.AddAsync(tarea, ct);

    public Task UpdateAsync(TareaMecanico tarea, CancellationToken ct = default)
    {
        _context.TareasMecanicos.Update(tarea);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TareaMecanico tarea, CancellationToken ct = default)
    {
        _context.TareasMecanicos.Remove(tarea);
        return Task.CompletedTask;
    }
}

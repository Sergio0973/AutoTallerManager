using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DetalleOrdenRepository : IDetalleOrdenRepository
{
    private readonly AutoTallerDbContext _context;

    public DetalleOrdenRepository(AutoTallerDbContext context) => _context = context;

    public Task<DetalleOrden?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.DetallesOrden.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<DetalleOrden?> GetByOrdenAndRepuestoAsync(int ordenId, int repuestoId, CancellationToken ct = default) =>
        _context.DetallesOrden.FirstOrDefaultAsync(d => d.OrdenId == ordenId && d.RepuestoId == repuestoId, ct);

    public async Task<IReadOnlyList<DetalleOrden>> GetAllAsync(CancellationToken ct = default) =>
        await _context.DetallesOrden.OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<DetalleOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.DetallesOrden.Where(d => d.OrdenId == ordenId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<DetalleOrden>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default) =>
        await _context.DetallesOrden.Where(d => d.RepuestoId == repuestoId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task AddAsync(DetalleOrden detalle, CancellationToken ct = default) =>
        await _context.DetallesOrden.AddAsync(detalle, ct);

    public Task UpdateAsync(DetalleOrden detalle, CancellationToken ct = default)
    {
        _context.DetallesOrden.Update(detalle);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(DetalleOrden detalle, CancellationToken ct = default)
    {
        _context.DetallesOrden.Remove(detalle);
        return Task.CompletedTask;
    }
}

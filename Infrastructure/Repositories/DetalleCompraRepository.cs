using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DetalleCompraRepository : IDetalleCompraRepository
{
    private readonly AutoTallerDbContext _context;

    public DetalleCompraRepository(AutoTallerDbContext context) => _context = context;

    public Task<DetalleCompra?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.DetallesCompra.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<DetalleCompra?> GetByCompraAndRepuestoAsync(int compraId, int repuestoId, CancellationToken ct = default) =>
        _context.DetallesCompra.FirstOrDefaultAsync(d => d.CompraId == compraId && d.RepuestoId == repuestoId, ct);

    public async Task<IReadOnlyList<DetalleCompra>> GetAllAsync(CancellationToken ct = default) =>
        await _context.DetallesCompra.OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<DetalleCompra>> GetByCompraIdAsync(int compraId, CancellationToken ct = default) =>
        await _context.DetallesCompra.Where(d => d.CompraId == compraId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<DetalleCompra>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default) =>
        await _context.DetallesCompra.Where(d => d.RepuestoId == repuestoId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task AddAsync(DetalleCompra detalle, CancellationToken ct = default) =>
        await _context.DetallesCompra.AddAsync(detalle, ct);

    public Task UpdateAsync(DetalleCompra detalle, CancellationToken ct = default)
    {
        _context.DetallesCompra.Update(detalle);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(DetalleCompra detalle, CancellationToken ct = default)
    {
        _context.DetallesCompra.Remove(detalle);
        return Task.CompletedTask;
    }
}

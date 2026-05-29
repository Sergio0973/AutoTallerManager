using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class RepuestoProveedorRepository : IRepuestoProveedorRepository
{
    private readonly AutoTallerDbContext _context;

    public RepuestoProveedorRepository(AutoTallerDbContext context) => _context = context;

    public Task<RepuestoProveedor?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.RepuestosProveedor.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<RepuestoProveedor?> GetByRepuestoAndProveedorAsync(int repuestoId, int proveedorId, CancellationToken ct = default) =>
        _context.RepuestosProveedor.FirstOrDefaultAsync(r => r.RepuestoId == repuestoId && r.ProveedorId == proveedorId, ct);

    public async Task<IReadOnlyList<RepuestoProveedor>> GetAllAsync(CancellationToken ct = default) =>
        await _context.RepuestosProveedor.OrderBy(r => r.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<RepuestoProveedor>> GetByRepuestoIdAsync(int repuestoId, CancellationToken ct = default) =>
        await _context.RepuestosProveedor.Where(r => r.RepuestoId == repuestoId).OrderBy(r => r.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<RepuestoProveedor>> GetByProveedorIdAsync(int proveedorId, CancellationToken ct = default) =>
        await _context.RepuestosProveedor.Where(r => r.ProveedorId == proveedorId).OrderBy(r => r.Id).ToListAsync(ct);

    public async Task AddAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default) =>
        await _context.RepuestosProveedor.AddAsync(repuestoProveedor, ct);

    public Task UpdateAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default)
    {
        _context.RepuestosProveedor.Update(repuestoProveedor);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(RepuestoProveedor repuestoProveedor, CancellationToken ct = default)
    {
        _context.RepuestosProveedor.Remove(repuestoProveedor);
        return Task.CompletedTask;
    }
}

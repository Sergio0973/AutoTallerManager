using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CompraRepository : ICompraRepository
{
    private readonly AutoTallerDbContext _context;

    public CompraRepository(AutoTallerDbContext context) => _context = context;

    public Task<Compra?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Compras.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Compra>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Compras.OrderByDescending(c => c.FechaCompra).ThenBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Compra>> GetByProveedorIdAsync(int proveedorId, CancellationToken ct = default) =>
        await _context.Compras.Where(c => c.ProveedorId == proveedorId).OrderByDescending(c => c.FechaCompra).ToListAsync(ct);

    public async Task<IReadOnlyList<Compra>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default) =>
        await _context.Compras.Where(c => c.UsuarioId == usuarioId).OrderByDescending(c => c.FechaCompra).ToListAsync(ct);

    public async Task<IReadOnlyList<Compra>> GetByEstadoAsync(string estado, CancellationToken ct = default)
    {
        var estadoCompra = Domain.ValueObjects.Compras.EstadoCompra.Create(estado);
        return await _context.Compras.Where(c => c.Estado == estadoCompra).OrderByDescending(c => c.FechaCompra).ToListAsync(ct);
    }

    public async Task AddAsync(Compra compra, CancellationToken ct = default) =>
        await _context.Compras.AddAsync(compra, ct);

    public Task UpdateAsync(Compra compra, CancellationToken ct = default)
    {
        _context.Compras.Update(compra);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Compra compra, CancellationToken ct = default)
    {
        _context.Compras.Remove(compra);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.DetallesCompra.AnyAsync(d => d.CompraId == id, ct)
            || await _context.LogsInventario.AnyAsync(l => l.CompraId == id, ct);
    }
}

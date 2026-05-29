using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Pagos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PagoRepository : IPagoRepository
{
    private readonly AutoTallerDbContext _context;

    public PagoRepository(AutoTallerDbContext context) => _context = context;

    public Task<Pago?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Pagos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Pagos.OrderByDescending(p => p.FechaPago).ThenByDescending(p => p.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Pago>> GetByFacturaIdAsync(int facturaId, CancellationToken ct = default) =>
        await _context.Pagos.Where(p => p.FacturaId == facturaId).OrderByDescending(p => p.FechaPago).ToListAsync(ct);

    public async Task<IReadOnlyList<Pago>> GetByMetodoPagoIdAsync(int metodoPagoId, CancellationToken ct = default) =>
        await _context.Pagos.Where(p => p.MetodoPagoId == metodoPagoId).OrderByDescending(p => p.FechaPago).ToListAsync(ct);

    public async Task<IReadOnlyList<Pago>> GetByEstadoAsync(EstadoPago estado, CancellationToken ct = default) =>
        await _context.Pagos.Where(p => p.Estado == estado).OrderByDescending(p => p.FechaPago).ToListAsync(ct);

    public async Task AddAsync(Pago pago, CancellationToken ct = default) =>
        await _context.Pagos.AddAsync(pago, ct);

    public Task UpdateAsync(Pago pago, CancellationToken ct = default)
    {
        _context.Pagos.Update(pago);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Pago pago, CancellationToken ct = default)
    {
        _context.Pagos.Remove(pago);
        return Task.CompletedTask;
    }
}

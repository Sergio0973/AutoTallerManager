using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.MetodoPagos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class MetodoPagoRepository : IMetodoPagoRepository
{
    private readonly AutoTallerDbContext _context;

    public MetodoPagoRepository(AutoTallerDbContext context) => _context = context;

    public Task<MetodoPago?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.MetodosPago.FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<MetodoPago?> GetByNombreAsync(NombreMetodoPago nombre, CancellationToken ct = default) =>
        _context.MetodosPago.FirstOrDefaultAsync(m => m.Nombre == nombre, ct);

    public async Task<IReadOnlyList<MetodoPago>> GetAllAsync(CancellationToken ct = default) =>
        await _context.MetodosPago.OrderBy(m => m.Id).ToListAsync(ct);

    public async Task AddAsync(MetodoPago metodo, CancellationToken ct = default) =>
        await _context.MetodosPago.AddAsync(metodo, ct);

    public Task UpdateAsync(MetodoPago metodo, CancellationToken ct = default)
    {
        _context.MetodosPago.Update(metodo);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(MetodoPago metodo, CancellationToken ct = default)
    {
        _context.MetodosPago.Remove(metodo);
        return Task.CompletedTask;
    }
}

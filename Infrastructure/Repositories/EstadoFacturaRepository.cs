using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.EstadoFacturas;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class EstadoFacturaRepository : IEstadoFacturaRepository
{
    private readonly AutoTallerDbContext _context;

    public EstadoFacturaRepository(AutoTallerDbContext context) => _context = context;

    public Task<EstadoFactura?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.EstadosFactura.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<EstadoFactura?> GetByNombreAsync(NombreEstadoFactura nombre, CancellationToken ct = default) =>
        _context.EstadosFactura.FirstOrDefaultAsync(e => e.Nombre == nombre, ct);

    public async Task<IReadOnlyList<EstadoFactura>> GetAllAsync(CancellationToken ct = default) =>
        await _context.EstadosFactura.OrderBy(e => e.Id).ToListAsync(ct);

    public async Task AddAsync(EstadoFactura estado, CancellationToken ct = default) =>
        await _context.EstadosFactura.AddAsync(estado, ct);

    public Task UpdateAsync(EstadoFactura estado, CancellationToken ct = default)
    {
        _context.EstadosFactura.Update(estado);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(EstadoFactura estado, CancellationToken ct = default)
    {
        _context.EstadosFactura.Remove(estado);
        return Task.CompletedTask;
    }
}

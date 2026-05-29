using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.EstadoOrdenes;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class EstadoOrdenRepository : IEstadoOrdenRepository
{
    private readonly AutoTallerDbContext _context;

    public EstadoOrdenRepository(AutoTallerDbContext context) => _context = context;

    public Task<EstadoOrden?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.EstadosOrden.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<EstadoOrden?> GetByNombreAsync(NombreEstado nombre, CancellationToken ct = default) =>
        _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == nombre, ct);

    public async Task<IReadOnlyList<EstadoOrden>> GetAllAsync(CancellationToken ct = default) =>
        await _context.EstadosOrden.OrderBy(e => e.Id).ToListAsync(ct);

    public async Task AddAsync(EstadoOrden estado, CancellationToken ct = default) =>
        await _context.EstadosOrden.AddAsync(estado, ct);

    public Task UpdateAsync(EstadoOrden estado, CancellationToken ct = default)
    {
        _context.EstadosOrden.Update(estado);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(EstadoOrden estado, CancellationToken ct = default)
    {
        _context.EstadosOrden.Remove(estado);
        return Task.CompletedTask;
    }
}

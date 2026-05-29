using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class HistorialKilometrajeRepository : IHistorialKilometrajeRepository
{
    private readonly AutoTallerDbContext _context;

    public HistorialKilometrajeRepository(AutoTallerDbContext context) => _context = context;

    public Task<HistorialKilometraje?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.HistorialesKilometraje.FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IReadOnlyList<HistorialKilometraje>> GetAllAsync(CancellationToken ct = default) =>
        await _context.HistorialesKilometraje.OrderByDescending(h => h.Fecha).ThenByDescending(h => h.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<HistorialKilometraje>> GetByVehiculoIdAsync(int vehiculoId, CancellationToken ct = default) =>
        await _context.HistorialesKilometraje.Where(h => h.VehiculoId == vehiculoId).OrderByDescending(h => h.Fecha).ThenByDescending(h => h.Id).ToListAsync(ct);

    public async Task AddAsync(HistorialKilometraje historial, CancellationToken ct = default) =>
        await _context.HistorialesKilometraje.AddAsync(historial, ct);

    public Task UpdateAsync(HistorialKilometraje historial, CancellationToken ct = default)
    {
        _context.HistorialesKilometraje.Update(historial);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(HistorialKilometraje historial, CancellationToken ct = default)
    {
        _context.HistorialesKilometraje.Remove(historial);
        return Task.CompletedTask;
    }
}

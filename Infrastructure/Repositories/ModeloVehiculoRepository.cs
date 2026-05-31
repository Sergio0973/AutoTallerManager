using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ModeloVehiculos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ModeloVehiculoRepository : IModeloVehiculoRepository
{
    private readonly AutoTallerDbContext _context;

    public ModeloVehiculoRepository(AutoTallerDbContext context) => _context = context;

    public Task<ModeloVehiculo?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.ModelosVehiculo.FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<ModeloVehiculo?> GetByMarcaAndNombreAsync(int marcaId, NombreModelo nombre, CancellationToken ct = default) =>
        _context.ModelosVehiculo.FirstOrDefaultAsync(m => m.MarcaId == marcaId && m.Nombre == nombre, ct);

    public async Task<IReadOnlyList<ModeloVehiculo>> GetAllAsync(CancellationToken ct = default) =>
        await _context.ModelosVehiculo.OrderBy(m => m.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<ModeloVehiculo>> GetByMarcaIdAsync(int marcaId, CancellationToken ct = default) =>
        await _context.ModelosVehiculo.Where(m => m.MarcaId == marcaId).OrderBy(m => m.Id).ToListAsync(ct);

    public async Task AddAsync(ModeloVehiculo modelo, CancellationToken ct = default) =>
        await _context.ModelosVehiculo.AddAsync(modelo, ct);

    public Task UpdateAsync(ModeloVehiculo modelo, CancellationToken ct = default)
    {
        _context.ModelosVehiculo.Update(modelo);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(ModeloVehiculo modelo, CancellationToken ct = default)
    {
        _context.ModelosVehiculo.Remove(modelo);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Vehiculos.AnyAsync(v => v.ModeloId == id, ct);
    }
}

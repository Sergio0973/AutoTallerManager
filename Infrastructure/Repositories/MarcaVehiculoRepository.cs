using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.MarcaVehiculos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class MarcaVehiculoRepository : IMarcaVehiculoRepository
{
    private readonly AutoTallerDbContext _context;

    public MarcaVehiculoRepository(AutoTallerDbContext context) => _context = context;

    public Task<MarcaVehiculo?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.MarcasVehiculo.FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<MarcaVehiculo?> GetByNombreAsync(NombreMarca nombre, CancellationToken ct = default) =>
        _context.MarcasVehiculo.FirstOrDefaultAsync(m => m.Nombre == nombre, ct);

    public async Task<IReadOnlyList<MarcaVehiculo>> GetAllAsync(CancellationToken ct = default) =>
        await _context.MarcasVehiculo.OrderBy(m => m.Id).ToListAsync(ct);

    public async Task AddAsync(MarcaVehiculo marca, CancellationToken ct = default) =>
        await _context.MarcasVehiculo.AddAsync(marca, ct);

    public Task UpdateAsync(MarcaVehiculo marca, CancellationToken ct = default)
    {
        _context.MarcasVehiculo.Update(marca);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(MarcaVehiculo marca, CancellationToken ct = default)
    {
        _context.MarcasVehiculo.Remove(marca);
        return Task.CompletedTask;
    }
}

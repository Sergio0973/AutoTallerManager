using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Ciudades;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CiudadRepository : ICiudadRepository
{
    private readonly AutoTallerDbContext _context;

    public CiudadRepository(AutoTallerDbContext context) => _context = context;

    public Task<Ciudad?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Ciudades.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Ciudad?> GetByDepartamentoAndNombreAsync(int departamentoId, NombreCiudad nombre, CancellationToken ct = default) =>
        _context.Ciudades.FirstOrDefaultAsync(c => c.DepartamentoId == departamentoId && c.Nombre == nombre, ct);

    public async Task<IReadOnlyList<Ciudad>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Ciudades.OrderBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Ciudad>> GetByDepartamentoIdAsync(int departamentoId, CancellationToken ct = default) =>
        await _context.Ciudades.Where(c => c.DepartamentoId == departamentoId).OrderBy(c => c.Id).ToListAsync(ct);

    public async Task AddAsync(Ciudad ciudad, CancellationToken ct = default) =>
        await _context.Ciudades.AddAsync(ciudad, ct);

    public Task UpdateAsync(Ciudad ciudad, CancellationToken ct = default)
    {
        _context.Ciudades.Update(ciudad);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Ciudad ciudad, CancellationToken ct = default)
    {
        _context.Ciudades.Remove(ciudad);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Proveedores.AnyAsync(p => p.CiudadId == id, ct)
            || await _context.ClienteDirecciones.AnyAsync(d => d.CiudadId == id, ct);
    }
}

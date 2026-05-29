using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Proveedores;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ProveedorRepository : IProveedorRepository
{
    private readonly AutoTallerDbContext _context;

    public ProveedorRepository(AutoTallerDbContext context) => _context = context;

    public Task<Proveedor?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Proveedores.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Proveedor?> GetByNitAsync(Nit nit, CancellationToken ct = default) =>
        _context.Proveedores.FirstOrDefaultAsync(p => p.Nit == nit, ct);

    public Task<Proveedor?> GetByCorreoAsync(CorreoProveedor correo, CancellationToken ct = default) =>
        _context.Proveedores.FirstOrDefaultAsync(p => p.Correo == correo, ct);

    public async Task<IReadOnlyList<Proveedor>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Proveedores.OrderBy(p => p.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Proveedor>> GetByCiudadIdAsync(int ciudadId, CancellationToken ct = default) =>
        await _context.Proveedores.Where(p => p.CiudadId == ciudadId).OrderBy(p => p.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Proveedor>> GetByActivoAsync(bool activo, CancellationToken ct = default) =>
        await _context.Proveedores.Where(p => p.Activo == activo).OrderBy(p => p.Id).ToListAsync(ct);

    public async Task AddAsync(Proveedor proveedor, CancellationToken ct = default) =>
        await _context.Proveedores.AddAsync(proveedor, ct);

    public Task UpdateAsync(Proveedor proveedor, CancellationToken ct = default)
    {
        _context.Proveedores.Update(proveedor);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Proveedor proveedor, CancellationToken ct = default)
    {
        _context.Proveedores.Remove(proveedor);
        return Task.CompletedTask;
    }
}

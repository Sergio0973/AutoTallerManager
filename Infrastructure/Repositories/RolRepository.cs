using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Roles;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class RolRepository : IRolRepository
{
    private readonly AutoTallerDbContext _context;

    public RolRepository(AutoTallerDbContext context) => _context = context;

    public Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<Rol?> GetByNombreAsync(NombreRol nombre, CancellationToken ct = default) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre, ct);

    public async Task<IReadOnlyList<Rol>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Roles.OrderBy(r => r.Id).ToListAsync(ct);

    public async Task AddAsync(Rol rol, CancellationToken ct = default) =>
        await _context.Roles.AddAsync(rol, ct);

    public Task UpdateAsync(Rol rol, CancellationToken ct = default)
    {
        _context.Roles.Update(rol);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Rol rol, CancellationToken ct = default)
    {
        _context.Roles.Remove(rol);
        return Task.CompletedTask;
    }

    public async Task<bool> HasUsuariosAsync(int id, CancellationToken ct = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.RolId == id, ct);
    }
}

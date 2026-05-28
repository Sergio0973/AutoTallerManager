using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Usuarios;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly AutoTallerDbContext _context;

    public UsuarioRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByCorreoAsync(CorreoUsuario correo, CancellationToken ct = default)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo, ct);
    }

    public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Usuarios.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Usuario>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default)
    {
        var query = _context.Usuarios.AsQueryable();

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        var query = _context.Usuarios.AsQueryable();
        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Usuario usuario, CancellationToken ct = default)
    {
        _context.Usuarios.Remove(usuario);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsCorreoAsync(CorreoUsuario correo, CancellationToken ct = default)
    {
        return await _context.Usuarios.AnyAsync(u => u.Correo == correo, ct);
    }
}

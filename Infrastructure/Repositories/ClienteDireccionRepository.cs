using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteDirecciones;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ClienteDireccionRepository : IClienteDireccionRepository
{
    private readonly AutoTallerDbContext _context;

    public ClienteDireccionRepository(AutoTallerDbContext context) => _context = context;

    public Task<ClienteDireccion?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.ClienteDirecciones.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<ClienteDireccion?> GetByClienteCiudadAndDireccionAsync(int clienteId, int ciudadId, DireccionFisica direccion, CancellationToken ct = default) =>
        _context.ClienteDirecciones.FirstOrDefaultAsync(d => d.ClienteId == clienteId && d.CiudadId == ciudadId && d.Direccion == direccion, ct);

    public async Task<IReadOnlyList<ClienteDireccion>> GetAllAsync(CancellationToken ct = default) =>
        await _context.ClienteDirecciones.OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<ClienteDireccion>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default) =>
        await _context.ClienteDirecciones.Where(d => d.ClienteId == clienteId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task AddAsync(ClienteDireccion direccion, CancellationToken ct = default) =>
        await _context.ClienteDirecciones.AddAsync(direccion, ct);

    public Task UpdateAsync(ClienteDireccion direccion, CancellationToken ct = default)
    {
        _context.ClienteDirecciones.Update(direccion);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(ClienteDireccion direccion, CancellationToken ct = default)
    {
        _context.ClienteDirecciones.Remove(direccion);
        return Task.CompletedTask;
    }
}

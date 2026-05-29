using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteTelefonos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ClienteTelefonoRepository : IClienteTelefonoRepository
{
    private readonly AutoTallerDbContext _context;

    public ClienteTelefonoRepository(AutoTallerDbContext context) => _context = context;

    public Task<ClienteTelefono?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.ClienteTelefonos.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<ClienteTelefono?> GetByClienteAndTelefonoAsync(int clienteId, NumeroTelefono telefono, CancellationToken ct = default) =>
        _context.ClienteTelefonos.FirstOrDefaultAsync(t => t.ClienteId == clienteId && t.Telefono == telefono, ct);

    public async Task<IReadOnlyList<ClienteTelefono>> GetAllAsync(CancellationToken ct = default) =>
        await _context.ClienteTelefonos.OrderBy(t => t.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<ClienteTelefono>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default) =>
        await _context.ClienteTelefonos.Where(t => t.ClienteId == clienteId).OrderBy(t => t.Id).ToListAsync(ct);

    public async Task AddAsync(ClienteTelefono telefono, CancellationToken ct = default) =>
        await _context.ClienteTelefonos.AddAsync(telefono, ct);

    public Task UpdateAsync(ClienteTelefono telefono, CancellationToken ct = default)
    {
        _context.ClienteTelefonos.Update(telefono);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(ClienteTelefono telefono, CancellationToken ct = default)
    {
        _context.ClienteTelefonos.Remove(telefono);
        return Task.CompletedTask;
    }
}

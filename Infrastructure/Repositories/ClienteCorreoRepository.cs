using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteCorreos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ClienteCorreoRepository : IClienteCorreoRepository
{
    private readonly AutoTallerDbContext _context;

    public ClienteCorreoRepository(AutoTallerDbContext context) => _context = context;

    public Task<ClienteCorreo?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.ClienteCorreos.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<ClienteCorreo?> GetByCorreoAsync(CorreoElectronico correo, CancellationToken ct = default) =>
        _context.ClienteCorreos.FirstOrDefaultAsync(c => c.Correo == correo, ct);

    public async Task<IReadOnlyList<ClienteCorreo>> GetAllAsync(CancellationToken ct = default) =>
        await _context.ClienteCorreos.OrderBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<ClienteCorreo>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default) =>
        await _context.ClienteCorreos.Where(c => c.ClienteId == clienteId).OrderBy(c => c.Id).ToListAsync(ct);

    public async Task AddAsync(ClienteCorreo correo, CancellationToken ct = default) =>
        await _context.ClienteCorreos.AddAsync(correo, ct);

    public Task UpdateAsync(ClienteCorreo correo, CancellationToken ct = default)
    {
        _context.ClienteCorreos.Update(correo);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(ClienteCorreo correo, CancellationToken ct = default)
    {
        _context.ClienteCorreos.Remove(correo);
        return Task.CompletedTask;
    }
}

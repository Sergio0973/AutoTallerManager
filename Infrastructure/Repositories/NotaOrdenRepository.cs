using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class NotaOrdenRepository : INotaOrdenRepository
{
    private readonly AutoTallerDbContext _context;

    public NotaOrdenRepository(AutoTallerDbContext context) => _context = context;

    public Task<NotaOrden?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.NotasOrden.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<IReadOnlyList<NotaOrden>> GetAllAsync(CancellationToken ct = default) =>
        await _context.NotasOrden.OrderByDescending(n => n.FechaNota).ToListAsync(ct);

    public async Task<IReadOnlyList<NotaOrden>> GetByOrdenIdAsync(int ordenId, CancellationToken ct = default) =>
        await _context.NotasOrden
            .Where(n => n.OrdenId == ordenId)
            .OrderByDescending(n => n.FechaNota)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<NotaOrden>> GetByUsuarioIdAsync(int usuarioId, CancellationToken ct = default) =>
        await _context.NotasOrden
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaNota)
            .ToListAsync(ct);

    public async Task AddAsync(NotaOrden nota, CancellationToken ct = default) =>
        await _context.NotasOrden.AddAsync(nota, ct);

    public Task UpdateAsync(NotaOrden nota, CancellationToken ct = default)
    {
        _context.NotasOrden.Update(nota);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(NotaOrden nota, CancellationToken ct = default)
    {
        _context.NotasOrden.Remove(nota);
        return Task.CompletedTask;
    }
}

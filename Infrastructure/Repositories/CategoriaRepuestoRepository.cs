using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.CategoriaRepuestos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CategoriaRepuestoRepository : ICategoriaRepuestoRepository
{
    private readonly AutoTallerDbContext _context;

    public CategoriaRepuestoRepository(AutoTallerDbContext context) => _context = context;

    public Task<CategoriaRepuesto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.CategoriasRepuesto.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<CategoriaRepuesto?> GetByNombreAsync(NombreCategoria nombre, CancellationToken ct = default) =>
        _context.CategoriasRepuesto.FirstOrDefaultAsync(c => c.Nombre == nombre, ct);

    public async Task<IReadOnlyList<CategoriaRepuesto>> GetAllAsync(CancellationToken ct = default) =>
        await _context.CategoriasRepuesto.OrderBy(c => c.Id).ToListAsync(ct);

    public async Task AddAsync(CategoriaRepuesto categoria, CancellationToken ct = default) =>
        await _context.CategoriasRepuesto.AddAsync(categoria, ct);

    public Task UpdateAsync(CategoriaRepuesto categoria, CancellationToken ct = default)
    {
        _context.CategoriasRepuesto.Update(categoria);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(CategoriaRepuesto categoria, CancellationToken ct = default)
    {
        _context.CategoriasRepuesto.Remove(categoria);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Repuestos.AnyAsync(r => r.CategoriaId == id, ct);
    }
}

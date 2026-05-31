using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Paises;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class PaisRepository : IPaisRepository
{
    private readonly AutoTallerDbContext _context;

    public PaisRepository(AutoTallerDbContext context) => _context = context;

    public Task<Pais?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Paises.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Pais?> GetByCodigoAsync(CodigoPais codigo, CancellationToken ct = default) =>
        _context.Paises.FirstOrDefaultAsync(p => p.Codigo == codigo, ct);

    public async Task<IReadOnlyList<Pais>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Paises.OrderBy(p => p.Id).ToListAsync(ct);

    public async Task AddAsync(Pais pais, CancellationToken ct = default) =>
        await _context.Paises.AddAsync(pais, ct);

    public Task UpdateAsync(Pais pais, CancellationToken ct = default)
    {
        _context.Paises.Update(pais);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Pais pais, CancellationToken ct = default)
    {
        _context.Paises.Remove(pais);
        return Task.CompletedTask;
    }

    public async Task<bool> HasDependenciesAsync(int id, CancellationToken ct = default)
    {
        return await _context.Departamentos.AnyAsync(d => d.PaisId == id, ct);
    }
}

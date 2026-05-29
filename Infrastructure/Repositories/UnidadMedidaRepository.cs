using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.UnidadMedidas;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UnidadMedidaRepository : IUnidadMedidaRepository
{
    private readonly AutoTallerDbContext _context;

    public UnidadMedidaRepository(AutoTallerDbContext context) => _context = context;

    public Task<UnidadMedida?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.UnidadesMedida.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<UnidadMedida?> GetByNombreAsync(NombreUnidad nombre, CancellationToken ct = default) =>
        _context.UnidadesMedida.FirstOrDefaultAsync(u => u.Nombre == nombre, ct);

    public Task<UnidadMedida?> GetByAbreviaturaAsync(Abreviatura abreviatura, CancellationToken ct = default) =>
        _context.UnidadesMedida.FirstOrDefaultAsync(u => u.Abreviatura == abreviatura, ct);

    public async Task<IReadOnlyList<UnidadMedida>> GetAllAsync(CancellationToken ct = default) =>
        await _context.UnidadesMedida.OrderBy(u => u.Id).ToListAsync(ct);

    public async Task AddAsync(UnidadMedida unidad, CancellationToken ct = default) =>
        await _context.UnidadesMedida.AddAsync(unidad, ct);

    public Task UpdateAsync(UnidadMedida unidad, CancellationToken ct = default)
    {
        _context.UnidadesMedida.Update(unidad);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(UnidadMedida unidad, CancellationToken ct = default)
    {
        _context.UnidadesMedida.Remove(unidad);
        return Task.CompletedTask;
    }
}

using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Departamentos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AutoTallerDbContext _context;

    public DepartamentoRepository(AutoTallerDbContext context) => _context = context;

    public Task<Departamento?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Departamentos.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<Departamento?> GetByPaisAndNombreAsync(int paisId, NombreDepartamento nombre, CancellationToken ct = default) =>
        _context.Departamentos.FirstOrDefaultAsync(d => d.PaisId == paisId && d.Nombre == nombre, ct);

    public async Task<IReadOnlyList<Departamento>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Departamentos.OrderBy(d => d.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Departamento>> GetByPaisIdAsync(int paisId, CancellationToken ct = default) =>
        await _context.Departamentos.Where(d => d.PaisId == paisId).OrderBy(d => d.Id).ToListAsync(ct);

    public async Task AddAsync(Departamento departamento, CancellationToken ct = default) =>
        await _context.Departamentos.AddAsync(departamento, ct);

    public Task UpdateAsync(Departamento departamento, CancellationToken ct = default)
    {
        _context.Departamentos.Update(departamento);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Departamento departamento, CancellationToken ct = default)
    {
        _context.Departamentos.Remove(departamento);
        return Task.CompletedTask;
    }
}

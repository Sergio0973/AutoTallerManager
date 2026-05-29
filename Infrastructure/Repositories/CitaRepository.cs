using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CitaRepository : ICitaRepository
{
    private readonly AutoTallerDbContext _context;

    public CitaRepository(AutoTallerDbContext context) => _context = context;

    public Task<Cita?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.Citas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Cita>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Citas.OrderBy(c => c.FechaCita).ThenBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Cita>> GetByVehiculoIdAsync(int vehiculoId, CancellationToken ct = default) =>
        await _context.Citas.Where(c => c.VehiculoId == vehiculoId).OrderBy(c => c.FechaCita).ThenBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Cita>> GetByRecepcionistaIdAsync(int recepcionistaId, CancellationToken ct = default) =>
        await _context.Citas.Where(c => c.RecepcionistaId == recepcionistaId).OrderBy(c => c.FechaCita).ThenBy(c => c.Id).ToListAsync(ct);

    public async Task<IReadOnlyList<Cita>> GetByTipoServicioIdAsync(int tipoServicioId, CancellationToken ct = default) =>
        await _context.Citas.Where(c => c.TipoServicioId == tipoServicioId).OrderBy(c => c.FechaCita).ThenBy(c => c.Id).ToListAsync(ct);

    public Task<bool> ExistsOverlapAsync(int vehiculoId, DateOnly fechaCita, TimeOnly horaInicio, TimeOnly horaFin, int? excludeId = null, CancellationToken ct = default)
    {
        var query = _context.Citas.Where(c =>
            c.VehiculoId == vehiculoId &&
            c.FechaCita == fechaCita &&
            c.Horario.HoraInicio < horaFin &&
            horaInicio < c.Horario.HoraFin);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return query.AnyAsync(ct);
    }

    public async Task AddAsync(Cita cita, CancellationToken ct = default) =>
        await _context.Citas.AddAsync(cita, ct);

    public Task UpdateAsync(Cita cita, CancellationToken ct = default)
    {
        _context.Citas.Update(cita);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Cita cita, CancellationToken ct = default)
    {
        _context.Citas.Remove(cita);
        return Task.CompletedTask;
    }
}

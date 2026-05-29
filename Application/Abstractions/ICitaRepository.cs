using Domain.Entities;

namespace Application.Abstractions;

public interface ICitaRepository
{
    Task<Cita?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetByVehiculoIdAsync(int vehiculoId, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetByRecepcionistaIdAsync(int recepcionistaId, CancellationToken ct = default);
    Task<IReadOnlyList<Cita>> GetByTipoServicioIdAsync(int tipoServicioId, CancellationToken ct = default);
    Task<bool> ExistsOverlapAsync(int vehiculoId, DateOnly fechaCita, TimeOnly horaInicio, TimeOnly horaFin, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Cita cita, CancellationToken ct = default);
    Task UpdateAsync(Cita cita, CancellationToken ct = default);
    Task RemoveAsync(Cita cita, CancellationToken ct = default);
}

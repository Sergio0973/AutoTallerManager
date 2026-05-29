using Domain.Entities;
using Domain.ValueObjects.Pagos;

namespace Application.Abstractions;

public interface IPagoRepository
{
    Task<Pago?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetByFacturaIdAsync(int facturaId, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetByMetodoPagoIdAsync(int metodoPagoId, CancellationToken ct = default);
    Task<IReadOnlyList<Pago>> GetByEstadoAsync(EstadoPago estado, CancellationToken ct = default);
    Task AddAsync(Pago pago, CancellationToken ct = default);
    Task UpdateAsync(Pago pago, CancellationToken ct = default);
    Task RemoveAsync(Pago pago, CancellationToken ct = default);
}

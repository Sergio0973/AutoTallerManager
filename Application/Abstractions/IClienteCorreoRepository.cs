using Domain.Entities;
using Domain.ValueObjects.ClienteCorreos;

namespace Application.Abstractions;

public interface IClienteCorreoRepository
{
    Task<ClienteCorreo?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ClienteCorreo?> GetByCorreoAsync(CorreoElectronico correo, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteCorreo>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClienteCorreo>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default);
    Task AddAsync(ClienteCorreo correo, CancellationToken ct = default);
    Task UpdateAsync(ClienteCorreo correo, CancellationToken ct = default);
    Task RemoveAsync(ClienteCorreo correo, CancellationToken ct = default);
}

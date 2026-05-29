using Domain.Entities;
using Domain.ValueObjects.ClienteTelefonos;

namespace Application.Abstractions;

public interface IClienteTelefonoRepository
{
    Task<ClienteTelefono?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ClienteTelefono?> GetByClienteAndTelefonoAsync(int clienteId, NumeroTelefono telefono, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteTelefono>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClienteTelefono>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default);
    Task AddAsync(ClienteTelefono telefono, CancellationToken ct = default);
    Task UpdateAsync(ClienteTelefono telefono, CancellationToken ct = default);
    Task RemoveAsync(ClienteTelefono telefono, CancellationToken ct = default);
}

using Domain.Entities;
using Domain.ValueObjects.ClienteDirecciones;

namespace Application.Abstractions;

public interface IClienteDireccionRepository
{
    Task<ClienteDireccion?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ClienteDireccion?> GetByClienteCiudadAndDireccionAsync(int clienteId, int ciudadId, DireccionFisica direccion, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteDireccion>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClienteDireccion>> GetByClienteIdAsync(int clienteId, CancellationToken ct = default);
    Task AddAsync(ClienteDireccion direccion, CancellationToken ct = default);
    Task UpdateAsync(ClienteDireccion direccion, CancellationToken ct = default);
    Task RemoveAsync(ClienteDireccion direccion, CancellationToken ct = default);
}

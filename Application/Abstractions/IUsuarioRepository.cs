using Domain.Entities;
using Domain.ValueObjects.Usuarios;

namespace Application.Abstractions;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> GetByCorreoAsync(CorreoUsuario correo, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);

    Task AddAsync(Usuario usuario, CancellationToken ct = default);
    Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
    Task RemoveAsync(Usuario usuario, CancellationToken ct = default);
    Task<bool> ExistsCorreoAsync(CorreoUsuario correo, CancellationToken ct = default);
}

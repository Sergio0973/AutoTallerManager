using System.Globalization;
using System.Text;
using Application.Abstractions;

namespace Application.Common.Security;

public static class UserRoleGuard
{
    public static async Task EnsureRecepcionistaAsync(
        IUnitOfWork uow,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureAnyRoleAsync(
            uow,
            usuarioId,
            "Recepcionista",
            [RoleNames.Recepcionista, RoleNames.Admin],
            cancellationToken);
    }

    public static async Task EnsureMecanicoAsync(
        IUnitOfWork uow,
        int usuarioId,
        CancellationToken cancellationToken)
    {
        await EnsureAnyRoleAsync(
            uow,
            usuarioId,
            "Mecanico",
            [RoleNames.Mecanico, RoleNames.Admin],
            cancellationToken);
    }

    public static async Task EnsureAnyRoleAsync(
        IUnitOfWork uow,
        int usuarioId,
        string usuarioNombre,
        IReadOnlyCollection<string> rolesPermitidos,
        CancellationToken cancellationToken)
    {
        var usuario = await uow.Usuarios.GetByIdAsync(usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException($"{usuarioNombre} no encontrado.");

        var rol = await uow.Roles.GetByIdAsync(usuario.RolId, cancellationToken)
            ?? throw new KeyNotFoundException("Rol del usuario no encontrado.");

        var rolActual = NormalizeRole(rol.Nombre.Value);
        var permitidos = rolesPermitidos.Select(NormalizeRole).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!permitidos.Contains(rolActual))
        {
            throw new InvalidOperationException(
                $"{usuarioNombre} no tiene un rol permitido. Roles requeridos: {string.Join(", ", rolesPermitidos)}.");
        }
    }

    private static string NormalizeRole(string value)
    {
        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}

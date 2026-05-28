using System.Net.Mail;

namespace Domain.ValueObjects.Usuarios;

public sealed record CorreoUsuario
{
    public string Value { get; }

    private CorreoUsuario(string value)
    {
        Value = value;
    }

    public static CorreoUsuario Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El correo de usuario es obligatorio.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalized, out _))
            throw new ArgumentException("El correo de usuario no tiene un formato válido.", nameof(value));

        return new CorreoUsuario(normalized);
    }

    public override string ToString() => Value;
}

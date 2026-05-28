using System.Net.Mail;

namespace Domain.ValueObjects.Proveedores;

public sealed record CorreoProveedor
{
    public string Value { get; }

    private CorreoProveedor(string value)
    {
        Value = value;
    }

    public static CorreoProveedor Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El correo del proveedor es obligatorio.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalized, out _))
            throw new ArgumentException("El correo del proveedor no tiene un formato válido.", nameof(value));

        return new CorreoProveedor(normalized);
    }

    public override string ToString() => Value;
}

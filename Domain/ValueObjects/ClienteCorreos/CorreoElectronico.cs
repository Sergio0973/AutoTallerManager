using System.Net.Mail;

namespace Domain.ValueObjects.ClienteCorreos;

public sealed record CorreoElectronico
{
    public string Value { get; }

    private CorreoElectronico(string value)
    {
        Value = value;
    }

    public static CorreoElectronico Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalized, out _))
            throw new ArgumentException("El correo electrónico no tiene un formato válido.", nameof(value));

        return new CorreoElectronico(normalized);
    }

    public override string ToString() => Value;
}

using System.Text.RegularExpressions;

namespace Domain.ValueObjects.ClienteTelefonos;

public sealed record NumeroTelefono
{
    public string Value { get; }

    private NumeroTelefono(string value)
    {
        Value = value;
    }

    public static NumeroTelefono Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El número de teléfono es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length < 7 || normalized.Length > 20)
            throw new ArgumentException("El número de teléfono debe tener entre 7 y 20 caracteres.", nameof(value));

        if (!Regex.IsMatch(normalized, @"^\+?[0-9]+$"))
            throw new ArgumentException("El número de teléfono solo puede contener dígitos y opcionalmente un '+' al inicio.", nameof(value));

        return new NumeroTelefono(normalized);
    }

    public override string ToString() => Value;
}

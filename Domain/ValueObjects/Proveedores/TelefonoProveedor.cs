using System.Text.RegularExpressions;

namespace Domain.ValueObjects.Proveedores;

public sealed record TelefonoProveedor
{
    public string Value { get; }

    private TelefonoProveedor(string value)
    {
        Value = value;
    }

    public static TelefonoProveedor Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El teléfono del proveedor es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length < 7 || normalized.Length > 20)
            throw new ArgumentException("El teléfono debe tener entre 7 y 20 caracteres.", nameof(value));

        if (!Regex.IsMatch(normalized, @"^\+?[0-9]+$"))
            throw new ArgumentException("El teléfono solo puede contener dígitos y opcionalmente un '+' al inicio.", nameof(value));

        return new TelefonoProveedor(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.ClienteTelefonos;

public sealed record TipoTelefono
{
    public string Value { get; }

    private TipoTelefono(string value)
    {
        Value = value;
    }

    public static TipoTelefono Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El tipo de teléfono es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El tipo de teléfono no puede exceder los 50 caracteres.", nameof(value));

        return new TipoTelefono(normalized);
    }

    public override string ToString() => Value;
}

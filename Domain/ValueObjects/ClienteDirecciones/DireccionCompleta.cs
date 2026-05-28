namespace Domain.ValueObjects.ClienteDirecciones;

public sealed record DireccionCompleta
{
    public string Value { get; }

    private DireccionCompleta(string value)
    {
        Value = value;
    }

    public static DireccionCompleta Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La dirección completa es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 255)
            throw new ArgumentException("La dirección completa no puede exceder los 255 caracteres.", nameof(value));

        return new DireccionCompleta(normalized);
    }

    public override string ToString() => Value;
}

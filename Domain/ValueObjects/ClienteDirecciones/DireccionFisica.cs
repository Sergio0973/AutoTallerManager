namespace Domain.ValueObjects.ClienteDirecciones;

public sealed record DireccionFisica
{
    public string Value { get; }

    private DireccionFisica(string value)
    {
        Value = value;
    }

    public static DireccionFisica Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La dirección física es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 255)
            throw new ArgumentException("La dirección física no puede exceder los 255 caracteres.", nameof(value));

        return new DireccionFisica(normalized);
    }

    public override string ToString() => Value;
}

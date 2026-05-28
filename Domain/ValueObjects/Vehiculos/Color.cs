namespace Domain.ValueObjects.Vehiculos;

public sealed record Color
{
    public string Value { get; }

    private Color(string value)
    {
        Value = value;
    }

    public static Color Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El color es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El color no puede exceder los 50 caracteres.", nameof(value));

        return new Color(normalized);
    }

    public override string ToString() => Value;
}

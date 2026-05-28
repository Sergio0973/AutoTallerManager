namespace Domain.ValueObjects.Vehiculos;

public sealed record Placa
{
    public string Value { get; }

    private Placa(string value)
    {
        Value = value;
    }

    public static Placa Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La placa es obligatoria.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 20)
            throw new ArgumentException("La placa no puede exceder los 20 caracteres.", nameof(value));

        return new Placa(normalized);
    }

    public override string ToString() => Value;
}

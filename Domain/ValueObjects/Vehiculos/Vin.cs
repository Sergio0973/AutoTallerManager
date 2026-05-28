namespace Domain.ValueObjects.Vehiculos;

public sealed record Vin
{
    public string Value { get; }

    private Vin(string value)
    {
        Value = value;
    }

    public static Vin Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El VIN es obligatorio.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length != 17)
            throw new ArgumentException("El VIN debe tener exactamente 17 caracteres.", nameof(value));

        return new Vin(normalized);
    }

    public override string ToString() => Value;
}

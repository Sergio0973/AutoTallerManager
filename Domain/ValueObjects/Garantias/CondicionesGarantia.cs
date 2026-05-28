namespace Domain.ValueObjects.Garantias;

public sealed record CondicionesGarantia
{
    public string Value { get; }

    private CondicionesGarantia(string value)
    {
        Value = value;
    }

    public static CondicionesGarantia Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Las condiciones de la garantía son obligatorias.", nameof(value));

        var normalized = value.Trim();

        return new CondicionesGarantia(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Repuestos;

public sealed record DescripcionRepuesto
{
    public string Value { get; }

    private DescripcionRepuesto(string value)
    {
        Value = value;
    }

    public static DescripcionRepuesto Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del repuesto es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionRepuesto(normalized);
    }

    public override string ToString() => Value;
}

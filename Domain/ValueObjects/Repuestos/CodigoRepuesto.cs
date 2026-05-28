namespace Domain.ValueObjects.Repuestos;

public sealed record CodigoRepuesto
{
    public string Value { get; }

    private CodigoRepuesto(string value)
    {
        Value = value;
    }

    public static CodigoRepuesto Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El código del repuesto es obligatorio.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 50)
            throw new ArgumentException("El código del repuesto no puede exceder los 50 caracteres.", nameof(value));

        return new CodigoRepuesto(normalized);
    }

    public override string ToString() => Value;
}

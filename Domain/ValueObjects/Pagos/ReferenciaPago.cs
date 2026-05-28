namespace Domain.ValueObjects.Pagos;

public sealed record ReferenciaPago
{
    public string Value { get; }

    private ReferenciaPago(string value)
    {
        Value = value;
    }

    public static ReferenciaPago Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La referencia del pago es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("La referencia no puede exceder los 100 caracteres.", nameof(value));

        return new ReferenciaPago(normalized);
    }

    public override string ToString() => Value;
}

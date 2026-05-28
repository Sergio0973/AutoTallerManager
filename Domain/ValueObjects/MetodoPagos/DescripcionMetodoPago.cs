namespace Domain.ValueObjects.MetodoPagos;

public sealed record DescripcionMetodoPago
{
    public string Value { get; }

    private DescripcionMetodoPago(string value)
    {
        Value = value;
    }

    public static DescripcionMetodoPago Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del método de pago es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionMetodoPago(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Pagos;

public sealed record EstadoPago
{
    public string Value { get; }

    private EstadoPago(string value)
    {
        Value = value;
    }

    public static EstadoPago Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El estado del pago es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El estado del pago no puede exceder los 50 caracteres.", nameof(value));

        return new EstadoPago(normalized);
    }

    public override string ToString() => Value;
}

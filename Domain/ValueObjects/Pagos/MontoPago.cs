namespace Domain.ValueObjects.Pagos;

public sealed record MontoPago
{
    public decimal Value { get; }

    private MontoPago(decimal value)
    {
        Value = value;
    }

    public static MontoPago Create(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor a cero.", nameof(value));

        return new MontoPago(value);
    }

    public override string ToString() => Value.ToString("C");
}

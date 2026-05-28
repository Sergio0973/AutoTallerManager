namespace Domain.ValueObjects.DetalleOrdenes;

public sealed record PrecioSnapshot
{
    public decimal Value { get; }

    private PrecioSnapshot(decimal value)
    {
        Value = value;
    }

    public static PrecioSnapshot Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El precio snapshot no puede ser negativo.", nameof(value));

        return new PrecioSnapshot(value);
    }

    public override string ToString() => Value.ToString("C");
}

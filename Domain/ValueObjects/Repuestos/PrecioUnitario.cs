namespace Domain.ValueObjects.Repuestos;

public sealed record PrecioUnitario
{
    public decimal Value { get; }

    private PrecioUnitario(decimal value)
    {
        Value = value;
    }

    public static PrecioUnitario Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(value));

        return new PrecioUnitario(value);
    }

    public override string ToString() => Value.ToString("C");
}

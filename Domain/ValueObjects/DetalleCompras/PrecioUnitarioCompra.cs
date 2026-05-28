namespace Domain.ValueObjects.DetalleCompras;

public sealed record PrecioUnitarioCompra
{
    public decimal Value { get; }

    private PrecioUnitarioCompra(decimal value)
    {
        Value = value;
    }

    public static PrecioUnitarioCompra Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El precio unitario de la compra no puede ser negativo.", nameof(value));

        return new PrecioUnitarioCompra(value);
    }

    public override string ToString() => Value.ToString("C");
}

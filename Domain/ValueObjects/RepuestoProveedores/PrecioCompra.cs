namespace Domain.ValueObjects.RepuestoProveedores;

public sealed record PrecioCompra
{
    public decimal Value { get; }

    private PrecioCompra(decimal value)
    {
        Value = value;
    }

    public static PrecioCompra Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El precio de compra no puede ser negativo.", nameof(value));

        return new PrecioCompra(value);
    }

    public override string ToString() => Value.ToString("C");
}

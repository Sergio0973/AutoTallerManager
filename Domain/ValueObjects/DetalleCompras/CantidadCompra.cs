namespace Domain.ValueObjects.DetalleCompras;

public sealed record CantidadCompra
{
    public int Value { get; }

    private CantidadCompra(int value)
    {
        Value = value;
    }

    public static CantidadCompra Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("La cantidad de compra debe ser mayor a cero.", nameof(value));

        return new CantidadCompra(value);
    }

    public override string ToString() => Value.ToString();
}

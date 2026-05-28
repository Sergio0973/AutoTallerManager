namespace Domain.ValueObjects.Compras;

public sealed record TotalCompra
{
    public decimal Value { get; }

    private TotalCompra(decimal value)
    {
        Value = value;
    }

    public static TotalCompra Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El total de la compra no puede ser negativo.", nameof(value));

        return new TotalCompra(value);
    }

    public override string ToString() => Value.ToString("C");
}

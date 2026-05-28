namespace Domain.ValueObjects.DetalleOrdenes;

public sealed record CantidadOrden
{
    public int Value { get; }

    private CantidadOrden(int value)
    {
        Value = value;
    }

    public static CantidadOrden Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("La cantidad de la orden debe ser mayor a cero.", nameof(value));

        return new CantidadOrden(value);
    }

    public override string ToString() => Value.ToString();
}

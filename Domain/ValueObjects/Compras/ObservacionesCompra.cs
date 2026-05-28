namespace Domain.ValueObjects.Compras;

public sealed record ObservacionesCompra
{
    public string Value { get; }

    private ObservacionesCompra(string value)
    {
        Value = value;
    }

    public static ObservacionesCompra Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Las observaciones de la compra son obligatorias.", nameof(value));

        var normalized = value.Trim();

        return new ObservacionesCompra(normalized);
    }

    public override string ToString() => Value;
}

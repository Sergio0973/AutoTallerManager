namespace Domain.ValueObjects.Compras;

public sealed record EstadoCompra
{
    public string Value { get; }

    private EstadoCompra(string value)
    {
        Value = value;
    }

    public static EstadoCompra Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El estado de la compra es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El estado de la compra no puede exceder los 50 caracteres.", nameof(value));

        return new EstadoCompra(normalized);
    }

    public override string ToString() => Value;
}

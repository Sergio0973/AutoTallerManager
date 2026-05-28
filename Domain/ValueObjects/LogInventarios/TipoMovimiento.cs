namespace Domain.ValueObjects.LogInventarios;

public sealed record TipoMovimiento
{
    public string Value { get; }

    private TipoMovimiento(string value)
    {
        Value = value;
    }

    public static TipoMovimiento Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El tipo de movimiento es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El tipo de movimiento no puede exceder los 50 caracteres.", nameof(value));

        return new TipoMovimiento(normalized);
    }

    public override string ToString() => Value;
}

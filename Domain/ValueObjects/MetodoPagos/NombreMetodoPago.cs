namespace Domain.ValueObjects.MetodoPagos;

public sealed record NombreMetodoPago
{
    public string Value { get; }

    private NombreMetodoPago(string value)
    {
        Value = value;
    }

    public static NombreMetodoPago Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del método de pago es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El nombre no puede exceder los 50 caracteres.", nameof(value));

        return new NombreMetodoPago(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Facturas;

public sealed record ObservacionesFactura
{
    public string Value { get; }

    private ObservacionesFactura(string value)
    {
        Value = value;
    }

    public static ObservacionesFactura Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Las observaciones de la factura son obligatorias.", nameof(value));

        var normalized = value.Trim();

        return new ObservacionesFactura(normalized);
    }

    public override string ToString() => Value;
}

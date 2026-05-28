namespace Domain.ValueObjects.EstadoFacturas;

public sealed record NombreEstadoFactura
{
    public string Value { get; }

    private NombreEstadoFactura(string value)
    {
        Value = value;
    }

    public static NombreEstadoFactura Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del estado de factura es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El nombre no puede exceder los 50 caracteres.", nameof(value));

        return new NombreEstadoFactura(normalized);
    }

    public override string ToString() => Value;
}

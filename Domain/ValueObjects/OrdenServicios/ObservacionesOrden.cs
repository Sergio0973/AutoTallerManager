namespace Domain.ValueObjects.OrdenServicios;

public sealed record ObservacionesOrden
{
    public string Value { get; }

    private ObservacionesOrden(string value)
    {
        Value = value;
    }

    public static ObservacionesOrden Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Las observaciones de la orden son obligatorias.", nameof(value));

        var normalized = value.Trim();

        return new ObservacionesOrden(normalized);
    }

    public override string ToString() => Value;
}

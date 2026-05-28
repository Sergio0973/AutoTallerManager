namespace Domain.ValueObjects.HistorialEstadoOrdenes;

public sealed record ObservacionHistorial
{
    public string Value { get; }

    private ObservacionHistorial(string value)
    {
        Value = value;
    }

    public static ObservacionHistorial Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La observación del historial es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new ObservacionHistorial(normalized);
    }

    public override string ToString() => Value;
}

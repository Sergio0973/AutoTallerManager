namespace Domain.ValueObjects.Citas;

public sealed record ObservacionesCita
{
    public string Value { get; }

    private ObservacionesCita(string value)
    {
        Value = value;
    }

    public static ObservacionesCita Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Las observaciones de la cita son obligatorias.", nameof(value));

        var normalized = value.Trim();

        return new ObservacionesCita(normalized);
    }

    public override string ToString() => Value;
}

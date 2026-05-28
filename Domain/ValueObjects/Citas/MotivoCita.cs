namespace Domain.ValueObjects.Citas;

public sealed record MotivoCita
{
    public string Value { get; }

    private MotivoCita(string value)
    {
        Value = value;
    }

    public static MotivoCita Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El motivo de la cita es obligatorio.", nameof(value));

        var normalized = value.Trim();

        return new MotivoCita(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Citas;

public sealed record EstadoCita
{
    public string Value { get; }

    private EstadoCita(string value)
    {
        Value = value;
    }

    public static EstadoCita Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El estado de la cita es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El estado de la cita no puede exceder los 50 caracteres.", nameof(value));

        return new EstadoCita(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.TareaMecanicos;

public sealed record EstadoTarea
{
    public string Value { get; }

    private EstadoTarea(string value)
    {
        Value = value;
    }

    public static EstadoTarea Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El estado de la tarea es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El estado de la tarea no puede exceder los 50 caracteres.", nameof(value));

        return new EstadoTarea(normalized);
    }

    public override string ToString() => Value;
}

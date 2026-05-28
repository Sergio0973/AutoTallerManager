namespace Domain.ValueObjects.TareaMecanicos;

public sealed record DescripcionTarea
{
    public string Value { get; }

    private DescripcionTarea(string value)
    {
        Value = value;
    }

    public static DescripcionTarea Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción de la tarea es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionTarea(normalized);
    }

    public override string ToString() => Value;
}

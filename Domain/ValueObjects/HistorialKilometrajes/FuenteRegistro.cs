namespace Domain.ValueObjects.HistorialKilometrajes;

public sealed record FuenteRegistro
{
    public string Value { get; }

    private FuenteRegistro(string value)
    {
        Value = value;
    }

    public static FuenteRegistro Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La fuente de registro es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("La fuente de registro no puede exceder los 100 caracteres.", nameof(value));

        return new FuenteRegistro(normalized);
    }

    public override string ToString() => Value;
}

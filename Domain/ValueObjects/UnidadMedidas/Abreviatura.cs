namespace Domain.ValueObjects.UnidadMedidas;

public sealed record Abreviatura
{
    public string Value { get; }

    private Abreviatura(string value)
    {
        Value = value;
    }

    public static Abreviatura Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La abreviatura es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 10)
            throw new ArgumentException("La abreviatura no puede exceder los 10 caracteres.", nameof(value));

        return new Abreviatura(normalized);
    }

    public override string ToString() => Value;
}

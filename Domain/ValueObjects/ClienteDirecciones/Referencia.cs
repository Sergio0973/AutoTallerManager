namespace Domain.ValueObjects.ClienteDirecciones;

public sealed record Referencia
{
    public string Value { get; }

    private Referencia(string value)
    {
        Value = value;
    }

    public static Referencia Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La referencia es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new Referencia(normalized);
    }

    public override string ToString() => Value;
}

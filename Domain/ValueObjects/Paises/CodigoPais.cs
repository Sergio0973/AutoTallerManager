namespace Domain.ValueObjects.Paises;

public sealed record CodigoPais
{
    public string Value { get; }

    private CodigoPais(string value)
    {
        Value = value;
    }

    public static CodigoPais Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El código del país es obligatorio.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > 10)
            throw new ArgumentException("El código del país no puede exceder los 10 caracteres.", nameof(value));

        return new CodigoPais(normalized);
    }

    public override string ToString() => Value;
}

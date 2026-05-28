namespace Domain.ValueObjects.Paises;

public sealed record NombrePais
{
    public string Value { get; }

    private NombrePais(string value)
    {
        Value = value;
    }

    public static NombrePais Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del país es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre del país no puede exceder los 100 caracteres.", nameof(value));

        return new NombrePais(normalized);
    }

    public override string ToString() => Value;
}

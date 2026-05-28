namespace Domain.ValueObjects.Clientes;

public sealed record Documento
{
    public string Value { get; }

    private Documento(string value)
    {
        Value = value;
    }

    public static Documento Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El documento es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length < 5 || normalized.Length > 20)
            throw new ArgumentException("El documento debe tener entre 5 y 20 caracteres.", nameof(value));

        return new Documento(normalized);
    }

    public override string ToString() => Value;
}

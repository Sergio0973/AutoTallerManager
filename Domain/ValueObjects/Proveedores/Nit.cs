namespace Domain.ValueObjects.Proveedores;

public sealed record Nit
{
    public string Value { get; }

    private Nit(string value)
    {
        Value = value;
    }

    public static Nit Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El NIT es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 20)
            throw new ArgumentException("El NIT no puede exceder los 20 caracteres.", nameof(value));

        return new Nit(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.ClienteDirecciones;

public sealed record CodigoPostal
{
    public string Value { get; }

    private CodigoPostal(string value)
    {
        Value = value;
    }

    public static CodigoPostal Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El código postal es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 20)
            throw new ArgumentException("El código postal no puede exceder los 20 caracteres.", nameof(value));

        return new CodigoPostal(normalized);
    }

    public override string ToString() => Value;
}

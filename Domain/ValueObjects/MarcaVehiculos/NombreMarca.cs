namespace Domain.ValueObjects.MarcaVehiculos;

public sealed record NombreMarca
{
    public string Value { get; }

    private NombreMarca(string value)
    {
        Value = value;
    }

    public static NombreMarca Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre de la marca es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre de la marca no puede exceder los 100 caracteres.", nameof(value));

        return new NombreMarca(normalized);
    }

    public override string ToString() => Value;
}

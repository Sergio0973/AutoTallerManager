namespace Domain.ValueObjects.Ciudades;

public sealed record NombreCiudad
{
    public string Value { get; }

    private NombreCiudad(string value)
    {
        Value = value;
    }

    public static NombreCiudad Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre de la ciudad es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre de la ciudad no puede exceder los 100 caracteres.", nameof(value));

        return new NombreCiudad(normalized);
    }

    public override string ToString() => Value;
}

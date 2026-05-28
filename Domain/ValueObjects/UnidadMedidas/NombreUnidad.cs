namespace Domain.ValueObjects.UnidadMedidas;

public sealed record NombreUnidad
{
    public string Value { get; }

    private NombreUnidad(string value)
    {
        Value = value;
    }

    public static NombreUnidad Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre de la unidad es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El nombre de la unidad no puede exceder los 50 caracteres.", nameof(value));

        return new NombreUnidad(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.ModeloVehiculos;

public sealed record NombreModelo
{
    public string Value { get; }

    private NombreModelo(string value)
    {
        Value = value;
    }

    public static NombreModelo Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del modelo es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre del modelo no puede exceder los 100 caracteres.", nameof(value));

        return new NombreModelo(normalized);
    }

    public override string ToString() => Value;
}

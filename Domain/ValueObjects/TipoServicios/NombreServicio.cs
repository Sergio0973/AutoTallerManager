namespace Domain.ValueObjects.TipoServicios;

public sealed record NombreServicio
{
    public string Value { get; }

    private NombreServicio(string value)
    {
        Value = value;
    }

    public static NombreServicio Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del servicio es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre del servicio no puede exceder los 100 caracteres.", nameof(value));

        return new NombreServicio(normalized);
    }

    public override string ToString() => Value;
}

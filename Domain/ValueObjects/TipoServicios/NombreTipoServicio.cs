namespace Domain.ValueObjects.TipoServicios;

public sealed record NombreTipoServicio
{
    public string Value { get; }

    private NombreTipoServicio(string value)
    {
        Value = value;
    }

    public static NombreTipoServicio Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del tipo de servicio es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre del tipo de servicio no puede exceder los 100 caracteres.", nameof(value));

        return new NombreTipoServicio(normalized);
    }

    public override string ToString() => Value;
}

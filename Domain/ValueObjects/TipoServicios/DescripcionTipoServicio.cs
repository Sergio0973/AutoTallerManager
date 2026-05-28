namespace Domain.ValueObjects.TipoServicios;

public sealed record DescripcionTipoServicio
{
    public string Value { get; }

    private DescripcionTipoServicio(string value)
    {
        Value = value;
    }

    public static DescripcionTipoServicio Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del tipo de servicio es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionTipoServicio(normalized);
    }

    public override string ToString() => Value;
}

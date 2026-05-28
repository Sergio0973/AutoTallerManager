namespace Domain.ValueObjects.TipoServicios;

public sealed record DescripcionServicio
{
    public string Value { get; }

    private DescripcionServicio(string value)
    {
        Value = value;
    }

    public static DescripcionServicio Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del servicio es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionServicio(normalized);
    }

    public override string ToString() => Value;
}

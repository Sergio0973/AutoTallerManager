namespace Domain.ValueObjects.EstadoOrdenes;

public sealed record DescripcionEstado
{
    public string Value { get; }

    private DescripcionEstado(string value)
    {
        Value = value;
    }

    public static DescripcionEstado Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del estado es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionEstado(normalized);
    }

    public override string ToString() => Value;
}

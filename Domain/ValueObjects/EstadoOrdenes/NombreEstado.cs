namespace Domain.ValueObjects.EstadoOrdenes;

public sealed record NombreEstado
{
    public string Value { get; }

    private NombreEstado(string value)
    {
        Value = value;
    }

    public static NombreEstado Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del estado es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El nombre del estado no puede exceder los 50 caracteres.", nameof(value));

        return new NombreEstado(normalized);
    }

    public override string ToString() => Value;
}

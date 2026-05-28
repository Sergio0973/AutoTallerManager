namespace Domain.ValueObjects.Auditorias;

public sealed record TipoAccion
{
    public string Value { get; }

    private TipoAccion(string value)
    {
        Value = value;
    }

    public static TipoAccion Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El tipo de acción es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El tipo de acción no puede exceder los 50 caracteres.", nameof(value));

        return new TipoAccion(normalized);
    }

    public override string ToString() => Value;
}

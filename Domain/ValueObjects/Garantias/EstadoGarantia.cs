namespace Domain.ValueObjects.Garantias;

public sealed record EstadoGarantia
{
    public string Value { get; }

    private EstadoGarantia(string value)
    {
        Value = value;
    }

    public static EstadoGarantia Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El estado de la garantía es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 50)
            throw new ArgumentException("El estado de la garantía no puede exceder los 50 caracteres.", nameof(value));

        return new EstadoGarantia(normalized);
    }

    public override string ToString() => Value;
}

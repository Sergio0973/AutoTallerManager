namespace Domain.ValueObjects.LogInventarios;

public sealed record MotivoMovimiento
{
    public string Value { get; }

    private MotivoMovimiento(string value)
    {
        Value = value;
    }

    public static MotivoMovimiento Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El motivo del movimiento es obligatorio.", nameof(value));

        var normalized = value.Trim();

        return new MotivoMovimiento(normalized);
    }

    public override string ToString() => Value;
}

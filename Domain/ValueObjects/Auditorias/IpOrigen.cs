namespace Domain.ValueObjects.Auditorias;

public sealed record IpOrigen
{
    public string Value { get; }

    private IpOrigen(string value)
    {
        Value = value;
    }

    public static IpOrigen Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La IP de origen es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 45) // IPv6 length
            throw new ArgumentException("La IP de origen no puede exceder los 45 caracteres.", nameof(value));

        return new IpOrigen(normalized);
    }

    public override string ToString() => Value;
}

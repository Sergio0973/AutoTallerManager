namespace Domain.ValueObjects.Auditorias;

public sealed record DetallesAuditoria
{
    public string Value { get; }

    private DetallesAuditoria(string value)
    {
        Value = value;
    }

    public static DetallesAuditoria Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Los detalles de la auditoría son obligatorios.", nameof(value));

        var normalized = value.Trim();

        return new DetallesAuditoria(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Auditorias;

public sealed record EntidadAuditada
{
    public string Value { get; }

    private EntidadAuditada(string value)
    {
        Value = value;
    }

    public static EntidadAuditada Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La entidad auditada es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("La entidad auditada no puede exceder los 100 caracteres.", nameof(value));

        return new EntidadAuditada(normalized);
    }

    public override string ToString() => Value;
}

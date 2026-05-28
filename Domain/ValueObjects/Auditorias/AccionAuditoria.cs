namespace Domain.ValueObjects.Auditorias;

public sealed record AccionAuditoria
{
    public string Value { get; }

    private AccionAuditoria(string value)
    {
        Value = value;
    }

    public static AccionAuditoria Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La acción de auditoría es obligatoria.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("La acción no puede exceder los 100 caracteres.", nameof(value));

        return new AccionAuditoria(normalized);
    }

    public override string ToString() => Value;
}

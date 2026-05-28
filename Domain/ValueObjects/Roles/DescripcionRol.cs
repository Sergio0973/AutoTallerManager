namespace Domain.ValueObjects.Roles;

public sealed record DescripcionRol
{
    public string Value { get; }

    private DescripcionRol(string value)
    {
        Value = value;
    }

    public static DescripcionRol Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción del rol es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionRol(normalized);
    }

    public override string ToString() => Value;
}

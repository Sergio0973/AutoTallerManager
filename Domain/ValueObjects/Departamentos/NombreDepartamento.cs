namespace Domain.ValueObjects.Departamentos;

public sealed record NombreDepartamento
{
    public string Value { get; }

    private NombreDepartamento(string value)
    {
        Value = value;
    }

    public static NombreDepartamento Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del departamento es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre del departamento no puede exceder los 100 caracteres.", nameof(value));

        return new NombreDepartamento(normalized);
    }

    public override string ToString() => Value;
}

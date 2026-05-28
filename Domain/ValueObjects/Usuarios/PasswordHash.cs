namespace Domain.ValueObjects.Usuarios;

public sealed record PasswordHash
{
    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El hash de la contraseña es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 255)
            throw new ArgumentException("El hash no puede exceder los 255 caracteres.", nameof(value));

        return new PasswordHash(normalized);
    }

    public override string ToString() => Value;
}

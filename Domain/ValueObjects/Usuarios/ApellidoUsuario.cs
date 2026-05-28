namespace Domain.ValueObjects.Usuarios;

public sealed record ApellidoUsuario
{
    public string Value { get; }

    private ApellidoUsuario(string value)
    {
        Value = value;
    }

    public static ApellidoUsuario Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El apellido de usuario es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El apellido de usuario no puede exceder los 100 caracteres.", nameof(value));

        return new ApellidoUsuario(normalized);
    }

    public override string ToString() => Value;
}

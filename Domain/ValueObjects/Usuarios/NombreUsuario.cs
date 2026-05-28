namespace Domain.ValueObjects.Usuarios;

public sealed record NombreUsuario
{
    public string Value { get; }

    private NombreUsuario(string value)
    {
        Value = value;
    }

    public static NombreUsuario Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre de usuario no puede exceder los 100 caracteres.", nameof(value));

        return new NombreUsuario(normalized);
    }

    public override string ToString() => Value;
}

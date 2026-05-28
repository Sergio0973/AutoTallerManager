namespace Domain.ValueObjects.CategoriaRepuestos;

public sealed record NombreCategoria
{
    public string Value { get; }

    private NombreCategoria(string value)
    {
        Value = value;
    }

    public static NombreCategoria Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 100)
            throw new ArgumentException("El nombre de la categoría no puede exceder los 100 caracteres.", nameof(value));

        return new NombreCategoria(normalized);
    }

    public override string ToString() => Value;
}

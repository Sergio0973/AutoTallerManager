namespace Domain.ValueObjects.CategoriaRepuestos;

public sealed record DescripcionCategoria
{
    public string Value { get; }

    private DescripcionCategoria(string value)
    {
        Value = value;
    }

    public static DescripcionCategoria Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción de la categoría es obligatoria.", nameof(value));

        var normalized = value.Trim();

        return new DescripcionCategoria(normalized);
    }

    public override string ToString() => Value;
}

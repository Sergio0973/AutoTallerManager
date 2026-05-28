namespace Domain.ValueObjects.Proveedores;

public sealed record NombreProveedor
{
    public string Value { get; }

    private NombreProveedor(string value)
    {
        Value = value;
    }

    public static NombreProveedor Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El nombre del proveedor es obligatorio.", nameof(value));

        var normalized = value.Trim();

        if (normalized.Length > 150)
            throw new ArgumentException("El nombre del proveedor no puede exceder los 150 caracteres.", nameof(value));

        return new NombreProveedor(normalized);
    }

    public override string ToString() => Value;
}

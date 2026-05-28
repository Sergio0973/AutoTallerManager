namespace Domain.ValueObjects.NotaOrdenes;

public sealed record ContenidoNota
{
    public string Value { get; }

    private ContenidoNota(string value)
    {
        Value = value;
    }

    public static ContenidoNota Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El contenido de la nota es obligatorio.", nameof(value));

        var normalized = value.Trim();

        return new ContenidoNota(normalized);
    }

    public override string ToString() => Value;
}

namespace Domain.ValueObjects.Auditorias;

public sealed record DatosJson
{
    public string Value { get; }

    private DatosJson(string value)
    {
        Value = value;
    }

    public static DatosJson Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Los datos JSON son obligatorios.", nameof(value));

        var normalized = value.Trim();

        return new DatosJson(normalized);
    }

    public override string ToString() => Value;
}

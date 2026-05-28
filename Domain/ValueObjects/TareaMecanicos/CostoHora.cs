namespace Domain.ValueObjects.TareaMecanicos;

public sealed record CostoHora
{
    public decimal Value { get; }

    private CostoHora(decimal value)
    {
        Value = value;
    }

    public static CostoHora Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("El costo por hora no puede ser negativo.", nameof(value));

        return new CostoHora(value);
    }

    public override string ToString() => Value.ToString("C");
}

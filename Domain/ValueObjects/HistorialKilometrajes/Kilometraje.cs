namespace Domain.ValueObjects.HistorialKilometrajes;

public sealed record Kilometraje
{
    public int Value { get; }

    private Kilometraje(int value)
    {
        Value = value;
    }

    public static Kilometraje Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("El kilometraje no puede ser negativo.", nameof(value));

        return new Kilometraje(value);
    }

    public override string ToString() => Value.ToString();
}

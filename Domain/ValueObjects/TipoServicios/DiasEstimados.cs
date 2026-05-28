namespace Domain.ValueObjects.TipoServicios;

public sealed record DiasEstimados
{
    public int Value { get; }

    private DiasEstimados(int value)
    {
        Value = value;
    }

    public static DiasEstimados Create(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Los días estimados deben ser mayores a cero.", nameof(value));

        return new DiasEstimados(value);
    }

    public override string ToString() => Value.ToString();
}

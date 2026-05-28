namespace Domain.ValueObjects.OrdenServicios;

public sealed record KilometrajeIngreso
{
    public int Value { get; }

    private KilometrajeIngreso(int value)
    {
        Value = value;
    }

    public static KilometrajeIngreso Create(int value)
    {
        if (value < 0)
            throw new ArgumentException("El kilometraje de ingreso no puede ser negativo.", nameof(value));

        return new KilometrajeIngreso(value);
    }

    public override string ToString() => Value.ToString();
}

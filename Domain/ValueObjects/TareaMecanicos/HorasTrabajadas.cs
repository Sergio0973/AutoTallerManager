namespace Domain.ValueObjects.TareaMecanicos;

public sealed record HorasTrabajadas
{
    public decimal Value { get; }

    private HorasTrabajadas(decimal value)
    {
        Value = value;
    }

    public static HorasTrabajadas Create(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Las horas trabajadas no pueden ser negativas.", nameof(value));

        return new HorasTrabajadas(value);
    }

    public override string ToString() => Value.ToString();
}

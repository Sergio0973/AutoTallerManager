namespace Domain.ValueObjects.Vehiculos;

public sealed record AnioVehiculo
{
    public short Value { get; }

    private AnioVehiculo(short value)
    {
        Value = value;
    }

    public static AnioVehiculo Create(short value)
    {
        if (value < 1900 || value > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("El año del vehículo no es válido.", nameof(value));

        return new AnioVehiculo(value);
    }

    public override string ToString() => Value.ToString();
}

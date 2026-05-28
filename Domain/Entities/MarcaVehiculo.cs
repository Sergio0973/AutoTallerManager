using Domain.Common;
using Domain.ValueObjects.MarcaVehiculos;

namespace Domain.Entities;

public sealed class MarcaVehiculo : BaseEntity<int>
{
    public NombreMarca Nombre { get; private set; } = default!;

    public ICollection<ModeloVehiculo> Modelos { get; private set; } = new List<ModeloVehiculo>();

    private MarcaVehiculo() { }

    public MarcaVehiculo(NombreMarca nombre)
    {
        Nombre = nombre;
    }

    public void Update(NombreMarca nombre)
    {
        Nombre = nombre;
    }
}

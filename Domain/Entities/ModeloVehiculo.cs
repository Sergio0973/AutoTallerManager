using Domain.Common;
using Domain.ValueObjects.ModeloVehiculos;

namespace Domain.Entities;

public sealed class ModeloVehiculo : BaseEntity<int>
{
    public int MarcaId { get; private set; }
    public NombreModelo Nombre { get; private set; } = default!;
    public RangoAnio Anios { get; private set; } = default!;

    public MarcaVehiculo? Marca { get; private set; }
    public ICollection<Vehiculo> Vehiculos { get; private set; } = new List<Vehiculo>();

    private ModeloVehiculo() { }

    public ModeloVehiculo(int marcaId, NombreModelo nombre, RangoAnio anios)
    {
        MarcaId = marcaId;
        Nombre = nombre;
        Anios = anios;
    }

    public void Update(int marcaId, NombreModelo nombre, RangoAnio anios)
    {
        MarcaId = marcaId;
        Nombre = nombre;
        Anios = anios;
    }
}

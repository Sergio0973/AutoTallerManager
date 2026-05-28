using Domain.Common;
using Domain.ValueObjects.Vehiculos;

namespace Domain.Entities;

public sealed class Vehiculo : BaseEntity<int>
{
    public int ClienteId { get; private set; }
    public int ModeloId { get; private set; }
    public Vin Vin { get; private set; } = default!;
    public AnioVehiculo Anio { get; private set; } = default!;
    public Placa Placa { get; private set; } = default!;
    public Color Color { get; private set; } = default!;
    public bool Activo { get; private set; }

    public Cliente? Cliente { get; private set; }
    public ModeloVehiculo? Modelo { get; private set; }
    public ICollection<HistorialKilometraje> HistorialKilometrajes { get; private set; } = new List<HistorialKilometraje>();
    public ICollection<Cita> Citas { get; private set; } = new List<Cita>();
    public ICollection<OrdenServicio> OrdeneServicio { get; private set; } = new List<OrdenServicio>();

    private Vehiculo() { }

    public Vehiculo(int clienteId, int modeloId, Vin vin, AnioVehiculo anio, Placa placa, Color color)
    {
        ClienteId = clienteId;
        ModeloId = modeloId;
        Vin = vin;
        Anio = anio;
        Placa = placa;
        Color = color;
        Activo = true;
    }

    public void Update(int clienteId, int modeloId, Vin vin, AnioVehiculo anio, Placa placa, Color color)
    {
        ClienteId = clienteId;
        ModeloId = modeloId;
        Vin = vin;
        Anio = anio;
        Placa = placa;
        Color = color;
    }

    public void CambiarEstado(bool activo)
    {
        Activo = activo;
    }
}

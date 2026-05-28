using Domain.Common;
using Domain.ValueObjects.HistorialKilometrajes;

namespace Domain.Entities;

public sealed class HistorialKilometraje : BaseEntity<int>
{
    public int VehiculoId { get; private set; }
    public Kilometraje Kilometraje { get; private set; } = default!;
    public DateOnly Fecha { get; private set; }
    public FuenteRegistro Fuente { get; private set; } = default!;

    public Vehiculo? Vehiculo { get; private set; }

    private HistorialKilometraje() { }

    public HistorialKilometraje(int vehiculoId, Kilometraje kilometraje, DateOnly fecha, FuenteRegistro fuente)
    {
        VehiculoId = vehiculoId;
        Kilometraje = kilometraje;
        Fecha = fecha;
        Fuente = fuente;
    }

    public void Update(int vehiculoId, Kilometraje kilometraje, DateOnly fecha, FuenteRegistro fuente)
    {
        VehiculoId = vehiculoId;
        Kilometraje = kilometraje;
        Fecha = fecha;
        Fuente = fuente;
    }
}

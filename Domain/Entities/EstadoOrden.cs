using Domain.Common;
using Domain.ValueObjects.EstadoOrdenes;

namespace Domain.Entities;

public sealed class EstadoOrden : BaseEntity<int>
{
    public NombreEstado Nombre { get; private set; } = default!;
    public DescripcionEstado Descripcion { get; private set; } = default!;

    public ICollection<OrdenServicio> OrdenesServicio { get; private set; } = new List<OrdenServicio>();
    public ICollection<HistorialEstadoOrden> Historiales { get; private set; } = new List<HistorialEstadoOrden>();

    private EstadoOrden() { }

    public EstadoOrden(NombreEstado nombre, DescripcionEstado descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Update(NombreEstado nombre, DescripcionEstado descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }
}

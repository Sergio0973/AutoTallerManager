using Domain.Common;
using Domain.ValueObjects.MetodoPagos;

namespace Domain.Entities;

public sealed class MetodoPago : BaseEntity<int>
{
    public NombreMetodoPago Nombre { get; private set; } = default!;
    public DescripcionMetodoPago Descripcion { get; private set; } = default!;

    public ICollection<Pago> Pagos { get; private set; } = new List<Pago>();

    private MetodoPago() { }

    public MetodoPago(NombreMetodoPago nombre, DescripcionMetodoPago descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Update(NombreMetodoPago nombre, DescripcionMetodoPago descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }
}

using Domain.Common;
using Domain.ValueObjects.EstadoFacturas;

namespace Domain.Entities;

public sealed class EstadoFactura : BaseEntity<int>
{
    public NombreEstadoFactura Nombre { get; private set; } = default!;

    public ICollection<Factura> Facturas { get; private set; } = new List<Factura>();

    private EstadoFactura() { }

    public EstadoFactura(NombreEstadoFactura nombre)
    {
        Nombre = nombre;
    }

    public void Update(NombreEstadoFactura nombre)
    {
        Nombre = nombre;
    }
}

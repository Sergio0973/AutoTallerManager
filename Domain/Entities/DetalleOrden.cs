using Domain.Common;
using Domain.ValueObjects.DetalleOrdenes;

namespace Domain.Entities;

public sealed class DetalleOrden : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int RepuestoId { get; private set; }
    public CantidadOrden Cantidad { get; private set; } = default!;
    public PrecioSnapshot PrecioSnapshot { get; private set; } = default!;

    public OrdenServicio? Orden { get; private set; }
    public Repuesto? Repuesto { get; private set; }

    private DetalleOrden() { }

    public DetalleOrden(int ordenId, int repuestoId, CantidadOrden cantidad, PrecioSnapshot precioSnapshot)
    {
        OrdenId = ordenId;
        RepuestoId = repuestoId;
        Cantidad = cantidad;
        PrecioSnapshot = precioSnapshot;
    }

    public void Update(CantidadOrden cantidad, PrecioSnapshot precioSnapshot)
    {
        Cantidad = cantidad;
        PrecioSnapshot = precioSnapshot;
    }
}

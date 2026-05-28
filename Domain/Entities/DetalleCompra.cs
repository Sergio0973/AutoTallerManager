using Domain.Common;
using Domain.ValueObjects.DetalleCompras;

namespace Domain.Entities;

public sealed class DetalleCompra : BaseEntity<int>
{
    public int CompraId { get; private set; }
    public int RepuestoId { get; private set; }
    public CantidadCompra Cantidad { get; private set; } = default!;
    public PrecioUnitarioCompra PrecioUnitario { get; private set; } = default!;

    public Compra? Compra { get; private set; }
    public Repuesto? Repuesto { get; private set; }

    private DetalleCompra() { }

    public DetalleCompra(int compraId, int repuestoId, CantidadCompra cantidad, PrecioUnitarioCompra precioUnitario)
    {
        CompraId = compraId;
        RepuestoId = repuestoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public void Update(CantidadCompra cantidad, PrecioUnitarioCompra precioUnitario)
    {
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }
}

using Domain.Common;
using Domain.ValueObjects.RepuestoProveedores;

namespace Domain.Entities;

public sealed class RepuestoProveedor : BaseEntity<int>
{
    public int RepuestoId { get; private set; }
    public int ProveedorId { get; private set; }
    public PrecioCompra PrecioCompra { get; private set; } = default!;
    public bool Principal { get; private set; }

    public Repuesto? Repuesto { get; private set; }
    public Proveedor? Proveedor { get; private set; }

    private RepuestoProveedor() { }

    public RepuestoProveedor(int repuestoId, int proveedorId, PrecioCompra precioCompra, bool principal)
    {
        RepuestoId = repuestoId;
        ProveedorId = proveedorId;
        PrecioCompra = precioCompra;
        Principal = principal;
    }

    public void Update(PrecioCompra precioCompra, bool principal)
    {
        PrecioCompra = precioCompra;
        Principal = principal;
    }
}

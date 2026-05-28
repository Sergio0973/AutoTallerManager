using Domain.Common;
using Domain.ValueObjects.Repuestos;

namespace Domain.Entities;

public sealed class Repuesto : BaseEntity<int>
{
    public int CategoriaId { get; private set; }
    public int UnidadId { get; private set; }
    public CodigoRepuesto Codigo { get; private set; } = default!;
    public DescripcionRepuesto Descripcion { get; private set; } = default!;
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public PrecioUnitario PrecioUnitario { get; private set; } = default!;
    public bool Activo { get; private set; }

    public CategoriaRepuesto? Categoria { get; private set; }
    public UnidadMedida? Unidad { get; private set; }
    public ICollection<RepuestoProveedor> Proveedores { get; private set; } = new List<RepuestoProveedor>();
    public ICollection<DetalleCompra> DetallesCompra { get; private set; } = new List<DetalleCompra>();
    public ICollection<DetalleOrden> DetallesOrden { get; private set; } = new List<DetalleOrden>();
    public ICollection<LogInventario> LogsInventario { get; private set; } = new List<LogInventario>();

    private Repuesto() { }

    public Repuesto(int categoriaId, int unidadId, CodigoRepuesto codigo, DescripcionRepuesto descripcion, int stockActual, int stockMinimo, PrecioUnitario precioUnitario)
    {
        CategoriaId = categoriaId;
        UnidadId = unidadId;
        Codigo = codigo;
        Descripcion = descripcion;
        StockActual = stockActual;
        StockMinimo = stockMinimo;
        PrecioUnitario = precioUnitario;
        Activo = true;
    }

    public void Update(int categoriaId, int unidadId, CodigoRepuesto codigo, DescripcionRepuesto descripcion, int stockActual, int stockMinimo, PrecioUnitario precioUnitario)
    {
        CategoriaId = categoriaId;
        UnidadId = unidadId;
        Codigo = codigo;
        Descripcion = descripcion;
        StockActual = stockActual;
        StockMinimo = stockMinimo;
        PrecioUnitario = precioUnitario;
    }

    public void CambiarEstado(bool activo)
    {
        Activo = activo;
    }
}

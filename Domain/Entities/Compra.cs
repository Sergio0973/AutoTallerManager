using Domain.Common;
using Domain.ValueObjects.Compras;

namespace Domain.Entities;

public sealed class Compra : BaseEntity<int>
{
    public int ProveedorId { get; private set; }
    public int UsuarioId { get; private set; }
    public DateOnly FechaCompra { get; private set; }
    public TotalCompra Total { get; private set; } = default!;
    public EstadoCompra Estado { get; private set; } = default!;
    public ObservacionesCompra? Observaciones { get; private set; }

    public Proveedor? Proveedor { get; private set; }
    public Usuario? Usuario { get; private set; }
    public ICollection<DetalleCompra> Detalles { get; private set; } = new List<DetalleCompra>();
    public ICollection<LogInventario> LogsInventario { get; private set; } = new List<LogInventario>();

    private Compra() { }

    public Compra(int proveedorId, int usuarioId, DateOnly fechaCompra, TotalCompra total, EstadoCompra estado, ObservacionesCompra? observaciones)
    {
        ProveedorId = proveedorId;
        UsuarioId = usuarioId;
        FechaCompra = fechaCompra;
        Total = total;
        Estado = estado;
        Observaciones = observaciones;
    }

    public void Update(int proveedorId, int usuarioId, DateOnly fechaCompra, TotalCompra total, EstadoCompra estado, ObservacionesCompra? observaciones)
    {
        ProveedorId = proveedorId;
        UsuarioId = usuarioId;
        FechaCompra = fechaCompra;
        Total = total;
        Estado = estado;
        Observaciones = observaciones;
    }
}

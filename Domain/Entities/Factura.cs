using Domain.Common;
using Domain.ValueObjects.Facturas;

namespace Domain.Entities;

public sealed class Factura : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int EstadoFacturaId { get; private set; }
    public int UsuarioId { get; private set; }
    public ValoresFactura Valores { get; private set; } = default!;
    public DateOnly FechaEmision { get; private set; }
    public ObservacionesFactura? Observaciones { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public EstadoFactura? EstadoFactura { get; private set; }
    public Usuario? Usuario { get; private set; }
    
    public ICollection<Pago> Pagos { get; private set; } = new List<Pago>();

    private Factura() { }

    public Factura(int ordenId, int estadoFacturaId, int usuarioId, ValoresFactura valores, DateOnly fechaEmision, ObservacionesFactura? observaciones)
    {
        OrdenId = ordenId;
        EstadoFacturaId = estadoFacturaId;
        UsuarioId = usuarioId;
        Valores = valores;
        FechaEmision = fechaEmision;
        Observaciones = observaciones;
    }

    public void Update(int estadoFacturaId, ValoresFactura valores, ObservacionesFactura? observaciones)
    {
        EstadoFacturaId = estadoFacturaId;
        Valores = valores;
        Observaciones = observaciones;
    }
}

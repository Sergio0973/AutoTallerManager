using Domain.Common;
using Domain.ValueObjects.Pagos;

namespace Domain.Entities;

public sealed class Pago : BaseEntity<int>
{
    public int FacturaId { get; private set; }
    public int MetodoPagoId { get; private set; }
    public MontoPago Monto { get; private set; } = default!;
    public DateTime FechaPago { get; private set; }
    public ReferenciaPago? Referencia { get; private set; }
    public EstadoPago Estado { get; private set; } = default!;

    public Factura? Factura { get; private set; }
    public MetodoPago? MetodoPago { get; private set; }

    private Pago() { }

    public Pago(int facturaId, int metodoPagoId, MontoPago monto, ReferenciaPago? referencia, EstadoPago estado)
    {
        FacturaId = facturaId;
        MetodoPagoId = metodoPagoId;
        Monto = monto;
        FechaPago = DateTime.UtcNow;
        Referencia = referencia;
        Estado = estado;
    }

    public void Update(EstadoPago estado)
    {
        Estado = estado;
    }
}

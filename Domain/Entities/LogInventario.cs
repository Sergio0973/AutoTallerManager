using Domain.Common;
using Domain.ValueObjects.LogInventarios;

namespace Domain.Entities;

public sealed class LogInventario : BaseEntity<long>
{
    public int RepuestoId { get; private set; }
    public int UsuarioId { get; private set; }
    public int? OrdenId { get; private set; }
    public int? CompraId { get; private set; }
    public TipoMovimiento TipoMovimiento { get; private set; } = default!;
    public int Cantidad { get; private set; }
    public int StockResultante { get; private set; }
    public DateTime Fecha { get; private set; }
    public MotivoMovimiento? Motivo { get; private set; }

    public Repuesto? Repuesto { get; private set; }
    public Usuario? Usuario { get; private set; }
    public OrdenServicio? Orden { get; private set; }
    public Compra? Compra { get; private set; }

    private LogInventario() { }

    public LogInventario(int repuestoId, int usuarioId, int? ordenId, int? compraId, TipoMovimiento tipoMovimiento, int cantidad, int stockResultante, MotivoMovimiento? motivo)
    {
        RepuestoId = repuestoId;
        UsuarioId = usuarioId;
        OrdenId = ordenId;
        CompraId = compraId;
        TipoMovimiento = tipoMovimiento;
        Cantidad = cantidad;
        StockResultante = stockResultante;
        Fecha = DateTime.UtcNow;
        Motivo = motivo;
    }
}

using Domain.Common;
using Domain.ValueObjects.HistorialEstadoOrdenes;

namespace Domain.Entities;

public sealed class HistorialEstadoOrden : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int EstadoId { get; private set; }
    public int UsuarioId { get; private set; }
    public DateTime FechaCambio { get; private set; }
    public ObservacionHistorial? Observacion { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public EstadoOrden? Estado { get; private set; }
    public Usuario? Usuario { get; private set; }

    private HistorialEstadoOrden() { }

    public HistorialEstadoOrden(int ordenId, int estadoId, int usuarioId, ObservacionHistorial? observacion)
    {
        OrdenId = ordenId;
        EstadoId = estadoId;
        UsuarioId = usuarioId;
        FechaCambio = DateTime.UtcNow;
        Observacion = observacion;
    }
}

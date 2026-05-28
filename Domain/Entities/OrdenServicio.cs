using Domain.Common;
using Domain.ValueObjects.OrdenServicios;

namespace Domain.Entities;

public sealed class OrdenServicio : BaseEntity<int>
{
    public int VehiculoId { get; private set; }
    public int RecepcionistaId { get; private set; }
    public int EstadoId { get; private set; }
    public int? CitaId { get; private set; }
    public KilometrajeIngreso KilometrajeIngreso { get; private set; } = default!;
    public DateOnly FechaIngreso { get; private set; }
    public DateOnly? FechaEstimada { get; private set; }
    public DateOnly? FechaEntregaReal { get; private set; }
    public ObservacionesOrden? Observaciones { get; private set; }

    public Vehiculo? Vehiculo { get; private set; }
    public Usuario? Recepcionista { get; private set; }
    public EstadoOrden? Estado { get; private set; }
    public Cita? Cita { get; private set; }
    public ICollection<OrdenTipoServicio> TiposServicio { get; private set; } = new List<OrdenTipoServicio>();
    public ICollection<OrdenMecanico> Mecanicos { get; private set; } = new List<OrdenMecanico>();
    public ICollection<TareaMecanico> Tareas { get; private set; } = new List<TareaMecanico>();
    public ICollection<NotaOrden> Notas { get; private set; } = new List<NotaOrden>();
    public ICollection<HistorialEstadoOrden> HistorialEstados { get; private set; } = new List<HistorialEstadoOrden>();
    public ICollection<DetalleOrden> Detalles { get; private set; } = new List<DetalleOrden>();
    public ICollection<LogInventario> LogsInventario { get; private set; } = new List<LogInventario>();
    public ICollection<Factura> Facturas { get; private set; } = new List<Factura>();
    public ICollection<Garantia> Garantias { get; private set; } = new List<Garantia>();

    private OrdenServicio() { }

    public OrdenServicio(int vehiculoId, int recepcionistaId, int estadoId, int? citaId, KilometrajeIngreso kilometrajeIngreso, DateOnly fechaIngreso, DateOnly? fechaEstimada, ObservacionesOrden? observaciones)
    {
        VehiculoId = vehiculoId;
        RecepcionistaId = recepcionistaId;
        EstadoId = estadoId;
        CitaId = citaId;
        KilometrajeIngreso = kilometrajeIngreso;
        FechaIngreso = fechaIngreso;
        FechaEstimada = fechaEstimada;
        Observaciones = observaciones;
    }

    public void Update(int vehiculoId, int recepcionistaId, int estadoId, int? citaId, KilometrajeIngreso kilometrajeIngreso, DateOnly? fechaEstimada, DateOnly? fechaEntregaReal, ObservacionesOrden? observaciones)
    {
        VehiculoId = vehiculoId;
        RecepcionistaId = recepcionistaId;
        EstadoId = estadoId;
        CitaId = citaId;
        KilometrajeIngreso = kilometrajeIngreso;
        FechaEstimada = fechaEstimada;
        FechaEntregaReal = fechaEntregaReal;
        Observaciones = observaciones;
    }
}

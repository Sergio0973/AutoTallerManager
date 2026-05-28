using Domain.Common;
using Domain.ValueObjects.TareaMecanicos;

namespace Domain.Entities;

public sealed class TareaMecanico : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int MecanicoId { get; private set; }
    public int TipoServicioId { get; private set; }
    public DescripcionTarea Descripcion { get; private set; } = default!;
    public HorasTrabajadas HorasTrabajadas { get; private set; } = default!;
    public CostoHora CostoHora { get; private set; } = default!;
    public EstadoTarea Estado { get; private set; } = default!;
    public DateTime? FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public Usuario? Mecanico { get; private set; }
    public TipoServicio? TipoServicio { get; private set; }

    private TareaMecanico() { }

    public TareaMecanico(int ordenId, int mecanicoId, int tipoServicioId, DescripcionTarea descripcion, HorasTrabajadas horasTrabajadas, CostoHora costoHora, EstadoTarea estado, DateTime? fechaInicio, DateTime? fechaFin)
    {
        OrdenId = ordenId;
        MecanicoId = mecanicoId;
        TipoServicioId = tipoServicioId;
        Descripcion = descripcion;
        HorasTrabajadas = horasTrabajadas;
        CostoHora = costoHora;
        Estado = estado;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public void Update(DescripcionTarea descripcion, HorasTrabajadas horasTrabajadas, CostoHora costoHora, EstadoTarea estado, DateTime? fechaInicio, DateTime? fechaFin)
    {
        Descripcion = descripcion;
        HorasTrabajadas = horasTrabajadas;
        CostoHora = costoHora;
        Estado = estado;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }
}

using Domain.Common;
using Domain.ValueObjects.Garantias;

namespace Domain.Entities;

public sealed class Garantia : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int TipoServicioId { get; private set; }
    public int MecanicoId { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaVencimiento { get; private set; }
    public CondicionesGarantia Condiciones { get; private set; } = default!;
    public EstadoGarantia Estado { get; private set; } = default!;

    public OrdenServicio? Orden { get; private set; }
    public TipoServicio? TipoServicio { get; private set; }
    public Usuario? Mecanico { get; private set; }

    private Garantia() { }

    public Garantia(int ordenId, int tipoServicioId, int mecanicoId, DateOnly fechaInicio, DateOnly fechaVencimiento, CondicionesGarantia condiciones, EstadoGarantia estado)
    {
        OrdenId = ordenId;
        TipoServicioId = tipoServicioId;
        MecanicoId = mecanicoId;
        FechaInicio = fechaInicio;
        FechaVencimiento = fechaVencimiento;
        Condiciones = condiciones;
        Estado = estado;
    }

    public void Update(EstadoGarantia estado)
    {
        Estado = estado;
    }
}

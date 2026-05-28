using Domain.Common;
using Domain.ValueObjects.Citas;

namespace Domain.Entities;

public sealed class Cita : BaseEntity<int>
{
    public int VehiculoId { get; private set; }
    public int RecepcionistaId { get; private set; }
    public int TipoServicioId { get; private set; }
    public DateOnly FechaCita { get; private set; }
    public HorarioCita Horario { get; private set; } = default!;
    public EstadoCita Estado { get; private set; } = default!;
    public ObservacionesCita? Observaciones { get; private set; }

    public Vehiculo? Vehiculo { get; private set; }
    public Usuario? Recepcionista { get; private set; }
    public TipoServicio? TipoServicio { get; private set; }

    private Cita() { }

    public Cita(int vehiculoId, int recepcionistaId, int tipoServicioId, DateOnly fechaCita, HorarioCita horario, EstadoCita estado, ObservacionesCita? observaciones)
    {
        VehiculoId = vehiculoId;
        RecepcionistaId = recepcionistaId;
        TipoServicioId = tipoServicioId;
        FechaCita = fechaCita;
        Horario = horario;
        Estado = estado;
        Observaciones = observaciones;
    }

    public void Update(int vehiculoId, int recepcionistaId, int tipoServicioId, DateOnly fechaCita, HorarioCita horario, EstadoCita estado, ObservacionesCita? observaciones)
    {
        VehiculoId = vehiculoId;
        RecepcionistaId = recepcionistaId;
        TipoServicioId = tipoServicioId;
        FechaCita = fechaCita;
        Horario = horario;
        Estado = estado;
        Observaciones = observaciones;
    }
}

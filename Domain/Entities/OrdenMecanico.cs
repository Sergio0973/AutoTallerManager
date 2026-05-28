using Domain.Common;

namespace Domain.Entities;

public sealed class OrdenMecanico : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int MecanicoId { get; private set; }
    public DateOnly FechaAsignacion { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public Usuario? Mecanico { get; private set; }

    private OrdenMecanico() { }

    public OrdenMecanico(int ordenId, int mecanicoId, DateOnly fechaAsignacion)
    {
        OrdenId = ordenId;
        MecanicoId = mecanicoId;
        FechaAsignacion = fechaAsignacion;
    }

    public void Update(DateOnly fechaAsignacion)
    {
        FechaAsignacion = fechaAsignacion;
    }
}

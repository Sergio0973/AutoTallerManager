using Domain.Common;
using Domain.ValueObjects.TipoServicios;

namespace Domain.Entities;

public sealed class TipoServicio : BaseEntity<int>
{
    public NombreTipoServicio Nombre { get; private set; } = default!;
    public DescripcionTipoServicio Descripcion { get; private set; } = default!;
    public DiasEstimados DiasEstimados { get; private set; } = default!;

    public ICollection<Cita> Citas { get; private set; } = new List<Cita>();
    public ICollection<Garantia> Garantias { get; private set; } = new List<Garantia>();

    private TipoServicio() { }

    public TipoServicio(NombreTipoServicio nombre, DescripcionTipoServicio descripcion, DiasEstimados diasEstimados)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        DiasEstimados = diasEstimados;
    }

    public void Update(NombreTipoServicio nombre, DescripcionTipoServicio descripcion, DiasEstimados diasEstimados)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        DiasEstimados = diasEstimados;
    }
}

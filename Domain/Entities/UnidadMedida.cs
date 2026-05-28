using Domain.Common;
using Domain.ValueObjects.UnidadMedidas;

namespace Domain.Entities;

public sealed class UnidadMedida : BaseEntity<int>
{
    public NombreUnidad Nombre { get; private set; } = default!;
    public Abreviatura Abreviatura { get; private set; } = default!;

    public ICollection<Repuesto> Repuestos { get; private set; } = new List<Repuesto>();

    private UnidadMedida() { }

    public UnidadMedida(NombreUnidad nombre, Abreviatura abreviatura)
    {
        Nombre = nombre;
        Abreviatura = abreviatura;
    }

    public void Update(NombreUnidad nombre, Abreviatura abreviatura)
    {
        Nombre = nombre;
        Abreviatura = abreviatura;
    }
}

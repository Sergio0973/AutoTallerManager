using Domain.Common;
using Domain.ValueObjects.Paises;

namespace Domain.Entities;

public sealed class Pais : BaseEntity<int>
{
    public NombrePais Nombre { get; private set; } = default!;
    public CodigoPais Codigo { get; private set; } = default!;

    public ICollection<Departamento> Departamentos { get; private set; } = new List<Departamento>();

    private Pais() { }

    public Pais(NombrePais nombre, CodigoPais codigo)
    {
        Nombre = nombre;
        Codigo = codigo;
    }

    public void Actualizar(NombrePais nombre, CodigoPais codigo)
    {
        Nombre = nombre;
        Codigo = codigo;
    }
}

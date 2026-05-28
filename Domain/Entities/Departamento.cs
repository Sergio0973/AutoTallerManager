using Domain.Common;
using Domain.ValueObjects.Departamentos;

namespace Domain.Entities;

public sealed class Departamento : BaseEntity<int>
{
    public int PaisId { get; private set; }
    public NombreDepartamento Nombre { get; private set; } = default!;

    public Pais? Pais { get; private set; }
    public ICollection<Ciudad> Ciudades { get; private set; } = new List<Ciudad>();

    private Departamento() { }

    public Departamento(int paisId, NombreDepartamento nombre)
    {
        PaisId = paisId;
        Nombre = nombre;
    }

    public void Update(int paisId, NombreDepartamento nombre)
    {
        PaisId = paisId;
        Nombre = nombre;
    }
}

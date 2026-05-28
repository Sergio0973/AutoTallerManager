using Domain.Common;
using Domain.ValueObjects.Ciudades;

namespace Domain.Entities;

public sealed class Ciudad : BaseEntity<int>
{
    public int DepartamentoId { get; private set; }
    public NombreCiudad Nombre { get; private set; } = default!;

    public Departamento? Departamento { get; private set; }
    public ICollection<ClienteDireccion> DireccionesCliente { get; private set; } = new List<ClienteDireccion>();
    public ICollection<Proveedor> Proveedores { get; private set; } = new List<Proveedor>();

    private Ciudad() { }

    public Ciudad(int departamentoId, NombreCiudad nombre)
    {
        DepartamentoId = departamentoId;
        Nombre = nombre;
    }

    public void Update(int departamentoId, NombreCiudad nombre)
    {
        DepartamentoId = departamentoId;
        Nombre = nombre;
    }
}

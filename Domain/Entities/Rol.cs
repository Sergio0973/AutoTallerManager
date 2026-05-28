using Domain.Common;
using Domain.ValueObjects.Roles;

namespace Domain.Entities;

public sealed class Rol : BaseEntity<int>
{
    public NombreRol Nombre { get; private set; } = default!;
    public DescripcionRol Descripcion { get; private set; } = default!;

    public ICollection<Usuario> Usuarios { get; private set; } = new List<Usuario>();

    private Rol() { }

    public Rol(NombreRol nombre, DescripcionRol descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Update(NombreRol nombre, DescripcionRol descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }
}

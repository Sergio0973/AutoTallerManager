using Domain.Common;
using Domain.ValueObjects.CategoriaRepuestos;

namespace Domain.Entities;

public sealed class CategoriaRepuesto : BaseEntity<int>
{
    public NombreCategoria Nombre { get; private set; } = default!;
    public DescripcionCategoria Descripcion { get; private set; } = default!;

    public ICollection<Repuesto> Repuestos { get; private set; } = new List<Repuesto>();

    private CategoriaRepuesto() { }

    public CategoriaRepuesto(NombreCategoria nombre, DescripcionCategoria descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void Update(NombreCategoria nombre, DescripcionCategoria descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
    }
}

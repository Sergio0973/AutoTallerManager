using Domain.Common;
using Domain.ValueObjects.Proveedores;

namespace Domain.Entities;

public sealed class Proveedor : BaseEntity<int>
{
    public NombreProveedor Nombre { get; private set; } = default!;
    public Nit Nit { get; private set; } = default!;
    public TelefonoProveedor Telefono { get; private set; } = default!;
    public CorreoProveedor Correo { get; private set; } = default!;
    public int CiudadId { get; private set; }
    public bool Activo { get; private set; }

    public Ciudad? Ciudad { get; private set; }
    public ICollection<Compra> Compras { get; private set; } = new List<Compra>();
    public ICollection<RepuestoProveedor> RepuestosProveedor { get; private set; } = new List<RepuestoProveedor>();

    private Proveedor() { }

    public Proveedor(NombreProveedor nombre, Nit nit, TelefonoProveedor telefono, CorreoProveedor correo, int ciudadId)
    {
        Nombre = nombre;
        Nit = nit;
        Telefono = telefono;
        Correo = correo;
        CiudadId = ciudadId;
        Activo = true;
    }

    public void Update(NombreProveedor nombre, Nit nit, TelefonoProveedor telefono, CorreoProveedor correo, int ciudadId)
    {
        Nombre = nombre;
        Nit = nit;
        Telefono = telefono;
        Correo = correo;
        CiudadId = ciudadId;
    }

    public void CambiarEstado(bool activo)
    {
        Activo = activo;
    }
}

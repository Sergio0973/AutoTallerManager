using Domain.Common;
using Domain.ValueObjects.Clientes;

namespace Domain.Entities;

public sealed class Cliente : BaseEntity<int>
{
    public NombreCompleto NombreCompleto { get; private set; } = default!;
    public Documento DocumentoIdentidad { get; private set; } = default!;
    public DateTime FechaRegistro { get; private set; }
    public bool Activo { get; private set; }

    public ICollection<ClienteTelefono> Telefonos { get; private set; } = new List<ClienteTelefono>();
    public ICollection<ClienteCorreo> Correos { get; private set; } = new List<ClienteCorreo>();
    public ICollection<ClienteDireccion> Direcciones { get; private set; } = new List<ClienteDireccion>();
    public ICollection<Vehiculo> Vehiculos { get; private set; } = new List<Vehiculo>();

    private Cliente() { }

    public Cliente(NombreCompleto nombreCompleto, Documento documentoIdentidad)
    {
        NombreCompleto = nombreCompleto;
        DocumentoIdentidad = documentoIdentidad;
        FechaRegistro = DateTime.UtcNow;
        Activo = true;
    }

    public void Actualizar(NombreCompleto nombreCompleto, Documento documentoIdentidad)
    {
        NombreCompleto = nombreCompleto;
        DocumentoIdentidad = documentoIdentidad;
    }

    public void CambiarEstado(bool activo)
    {
        Activo = activo;
    }
}

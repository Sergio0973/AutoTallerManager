using Domain.Common;
using Domain.ValueObjects.ClienteDirecciones;

namespace Domain.Entities;

public sealed class ClienteDireccion : BaseEntity<int>
{
    public int ClienteId { get; private set; }
    public int CiudadId { get; private set; }
    public DireccionFisica Direccion { get; private set; } = default!;
    public bool Principal { get; private set; }

    public Cliente? Cliente { get; private set; }
    public Ciudad? Ciudad { get; private set; }

    private ClienteDireccion() { }

    public ClienteDireccion(int clienteId, int ciudadId, DireccionFisica direccion, bool principal)
    {
        ClienteId = clienteId;
        CiudadId = ciudadId;
        Direccion = direccion;
        Principal = principal;
    }

    public void Update(int clienteId, int ciudadId, DireccionFisica direccion, bool principal)
    {
        ClienteId = clienteId;
        CiudadId = ciudadId;
        Direccion = direccion;
        Principal = principal;
    }
}

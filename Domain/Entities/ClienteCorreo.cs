using Domain.Common;
using Domain.ValueObjects.ClienteCorreos;

namespace Domain.Entities;

public sealed class ClienteCorreo : BaseEntity<int>
{
    public int ClienteId { get; private set; }
    public CorreoElectronico Correo { get; private set; } = default!;
    public bool Principal { get; private set; }

    public Cliente? Cliente { get; private set; }

    private ClienteCorreo() { }

    public ClienteCorreo(int clienteId, CorreoElectronico correo, bool principal)
    {
        ClienteId = clienteId;
        Correo = correo;
        Principal = principal;
    }

    public void Update(int clienteId, CorreoElectronico correo, bool principal)
    {
        ClienteId = clienteId;
        Correo = correo;
        Principal = principal;
    }
}

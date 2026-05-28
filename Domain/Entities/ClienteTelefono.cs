using Domain.Common;
using Domain.ValueObjects.ClienteTelefonos;

namespace Domain.Entities;

public sealed class ClienteTelefono : BaseEntity<int>
{
    public int ClienteId { get; private set; }
    public NumeroTelefono Telefono { get; private set; } = default!;
    public TipoTelefono Tipo { get; private set; } = default!;

    public Cliente? Cliente { get; private set; }

    private ClienteTelefono() { }

    public ClienteTelefono(int clienteId, NumeroTelefono telefono, TipoTelefono tipo)
    {
        ClienteId = clienteId;
        Telefono = telefono;
        Tipo = tipo;
    }

    public void Update(int clienteId, NumeroTelefono telefono, TipoTelefono tipo)
    {
        ClienteId = clienteId;
        Telefono = telefono;
        Tipo = tipo;
    }
}

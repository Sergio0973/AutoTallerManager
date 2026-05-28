using Domain.Common;
using Domain.ValueObjects.NotaOrdenes;

namespace Domain.Entities;

public sealed class NotaOrden : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int UsuarioId { get; private set; }
    public ContenidoNota Contenido { get; private set; } = default!;
    public DateTime FechaNota { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public Usuario? Usuario { get; private set; }

    private NotaOrden() { }

    public NotaOrden(int ordenId, int usuarioId, ContenidoNota contenido)
    {
        OrdenId = ordenId;
        UsuarioId = usuarioId;
        Contenido = contenido;
        FechaNota = DateTime.UtcNow;
    }

    public void Update(ContenidoNota contenido)
    {
        Contenido = contenido;
    }
}

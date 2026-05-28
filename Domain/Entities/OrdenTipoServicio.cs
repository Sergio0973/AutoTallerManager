using Domain.Common;

namespace Domain.Entities;

public sealed class OrdenTipoServicio : BaseEntity<int>
{
    public int OrdenId { get; private set; }
    public int TipoServicioId { get; private set; }

    public OrdenServicio? Orden { get; private set; }
    public TipoServicio? TipoServicio { get; private set; }

    private OrdenTipoServicio() { }

    public OrdenTipoServicio(int ordenId, int tipoServicioId)
    {
        OrdenId = ordenId;
        TipoServicioId = tipoServicioId;
    }
}

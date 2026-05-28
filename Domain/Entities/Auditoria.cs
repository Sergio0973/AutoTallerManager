using Domain.Common;
using Domain.ValueObjects.Auditorias;

namespace Domain.Entities;

public sealed class Auditoria : BaseEntity<long>
{
    public int UsuarioId { get; private set; }
    public EntidadAuditada Entidad { get; private set; } = default!;
    public int EntidadId { get; private set; }
    public TipoAccion TipoAccion { get; private set; } = default!;
    public DatosJson? DatosAnteriores { get; private set; }
    public DatosJson? DatosNuevos { get; private set; }
    public IpOrigen IpOrigen { get; private set; } = default!;
    public DateTime Fecha { get; private set; }

    public Usuario? Usuario { get; private set; }

    private Auditoria() { }

    public Auditoria(int usuarioId, EntidadAuditada entidad, int entidadId, TipoAccion tipoAccion, DatosJson? datosAnteriores, DatosJson? datosNuevos, IpOrigen ipOrigen)
    {
        UsuarioId = usuarioId;
        Entidad = entidad;
        EntidadId = entidadId;
        TipoAccion = tipoAccion;
        DatosAnteriores = datosAnteriores;
        DatosNuevos = datosNuevos;
        IpOrigen = ipOrigen;
        Fecha = DateTime.UtcNow;
    }
}

using Domain.Common;
using Domain.ValueObjects.Usuarios;

namespace Domain.Entities;

public sealed class Usuario : BaseEntity<int>
{
    public int RolId { get; private set; }
    public CorreoUsuario Correo { get; private set; } = default!;
    public NombreUsuario Nombre { get; private set; } = default!;
    public bool Activo { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    public Rol? Rol { get; private set; }
    public ICollection<Auditoria> Auditorias { get; private set; } = new List<Auditoria>();
    public ICollection<Cita> Citas { get; private set; } = new List<Cita>();
    public ICollection<OrdenServicio> OrdenesServicio { get; private set; } = new List<OrdenServicio>();
    public ICollection<Compra> Compras { get; private set; } = new List<Compra>();
    public ICollection<OrdenMecanico> AsignacionesMecanico { get; private set; } = new List<OrdenMecanico>();
    public ICollection<TareaMecanico> Tareas { get; private set; } = new List<TareaMecanico>();
    public ICollection<NotaOrden> Notas { get; private set; } = new List<NotaOrden>();
    public ICollection<HistorialEstadoOrden> HistorialEstados { get; private set; } = new List<HistorialEstadoOrden>();
    public ICollection<LogInventario> LogsInventario { get; private set; } = new List<LogInventario>();
    public ICollection<Factura> FacturasEmitidas { get; private set; } = new List<Factura>();
    public ICollection<Garantia> GarantiasMecanico { get; private set; } = new List<Garantia>();

    private Usuario() { }

    public Usuario(int rolId, CorreoUsuario correo, NombreUsuario nombre)
    {
        RolId = rolId;
        Correo = correo;
        Nombre = nombre;
        Activo = true;
        FechaCreacion = DateTime.UtcNow;
    }

    public void Update(int rolId, CorreoUsuario correo, NombreUsuario nombre)
    {
        RolId = rolId;
        Correo = correo;
        Nombre = nombre;
    }

    public void CambiarEstado(bool activo)
    {
        Activo = activo;
    }
}

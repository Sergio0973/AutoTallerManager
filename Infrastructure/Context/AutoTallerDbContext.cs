using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Context;

public class AutoTallerDbContext : DbContext
{
    public AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ClienteCorreo> ClienteCorreos => Set<ClienteCorreo>();
    public DbSet<ClienteDireccion> ClienteDirecciones => Set<ClienteDireccion>();
    public DbSet<ClienteTelefono> ClienteTelefonos => Set<ClienteTelefono>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<OrdenMecanico> OrdenesMecanicos => Set<OrdenMecanico>();
    public DbSet<OrdenTipoServicio> OrdenesTiposServicio => Set<OrdenTipoServicio>();
    public DbSet<DetalleOrden> DetallesOrden => Set<DetalleOrden>();
    public DbSet<TareaMecanico> TareasMecanicos => Set<TareaMecanico>();
    public DbSet<NotaOrden> NotasOrden => Set<NotaOrden>();
    public DbSet<HistorialEstadoOrden> HistorialEstadosOrden => Set<HistorialEstadoOrden>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<CategoriaRepuesto> CategoriasRepuesto => Set<CategoriaRepuesto>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<RepuestoProveedor> RepuestosProveedor => Set<RepuestoProveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<DetalleCompra> DetallesCompra => Set<DetalleCompra>();
    public DbSet<LogInventario> LogsInventario => Set<LogInventario>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<EstadoFactura> EstadosFactura => Set<EstadoFactura>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<Garantia> Garantias => Set<Garantia>();
    public DbSet<HistorialKilometraje> HistorialesKilometraje => Set<HistorialKilometraje>();
    public DbSet<MarcaVehiculo> MarcasVehiculo => Set<MarcaVehiculo>();
    public DbSet<ModeloVehiculo> ModelosVehiculo => Set<ModeloVehiculo>();
    public DbSet<Pais> Paises => Set<Pais>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplySingleValueObjectConversions();
        base.OnModelCreating(modelBuilder);
    }
}

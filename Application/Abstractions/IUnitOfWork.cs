namespace Application.Abstractions;

public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    IVehiculoRepository Vehiculos { get; }
    IOrdenServicioRepository OrdenesServicio { get; }
    IRepuestoRepository Repuestos { get; }
    IFacturaRepository Facturas { get; }
    IUsuarioRepository Usuarios { get; }
    IRolRepository Roles { get; }
    IEstadoOrdenRepository EstadosOrden { get; }
    IEstadoFacturaRepository EstadosFactura { get; }
    IMetodoPagoRepository MetodosPago { get; }
    IUnidadMedidaRepository UnidadesMedida { get; }
    ICategoriaRepuestoRepository CategoriasRepuesto { get; }
    ITipoServicioRepository TiposServicio { get; }
    IPaisRepository Paises { get; }
    IDepartamentoRepository Departamentos { get; }
    ICiudadRepository Ciudades { get; }
    IMarcaVehiculoRepository MarcasVehiculo { get; }
    IModeloVehiculoRepository ModelosVehiculo { get; }
    IClienteCorreoRepository ClienteCorreos { get; }
    IClienteTelefonoRepository ClienteTelefonos { get; }
    IClienteDireccionRepository ClienteDirecciones { get; }
    ICitaRepository Citas { get; }
    IProveedorRepository Proveedores { get; }
    IRepuestoProveedorRepository RepuestosProveedor { get; }
    ICompraRepository Compras { get; }
    IDetalleCompraRepository DetallesCompra { get; }
    ILogInventarioRepository LogsInventario { get; }
    IPagoRepository Pagos { get; }
    IGarantiaRepository Garantias { get; }
    IHistorialKilometrajeRepository HistorialesKilometraje { get; }
    IOrdenTipoServicioRepository OrdenesTiposServicio { get; }
    IOrdenMecanicoRepository OrdenesMecanicos { get; }
    IDetalleOrdenRepository DetallesOrden { get; }
    ITareaMecanicoRepository TareasMecanicos { get; }
    INotaOrdenRepository NotasOrden { get; }
    IHistorialEstadoOrdenRepository HistorialEstadosOrden { get; }
    IAuditoriaRepository Auditorias { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}

using Application.Abstractions;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AutoTallerDbContext _context;

    public IClienteRepository Clientes { get; }
    public IVehiculoRepository Vehiculos { get; }
    public IOrdenServicioRepository OrdenesServicio { get; }
    public IRepuestoRepository Repuestos { get; }
    public IFacturaRepository Facturas { get; }
    public IUsuarioRepository Usuarios { get; }
    public IRolRepository Roles { get; }
    public IEstadoOrdenRepository EstadosOrden { get; }
    public IEstadoFacturaRepository EstadosFactura { get; }
    public IMetodoPagoRepository MetodosPago { get; }
    public IUnidadMedidaRepository UnidadesMedida { get; }
    public ICategoriaRepuestoRepository CategoriasRepuesto { get; }
    public ITipoServicioRepository TiposServicio { get; }
    public IPaisRepository Paises { get; }
    public IDepartamentoRepository Departamentos { get; }
    public ICiudadRepository Ciudades { get; }
    public IMarcaVehiculoRepository MarcasVehiculo { get; }
    public IModeloVehiculoRepository ModelosVehiculo { get; }
    public IClienteCorreoRepository ClienteCorreos { get; }
    public IClienteTelefonoRepository ClienteTelefonos { get; }
    public IClienteDireccionRepository ClienteDirecciones { get; }
    public ICitaRepository Citas { get; }
    public IProveedorRepository Proveedores { get; }
    public IRepuestoProveedorRepository RepuestosProveedor { get; }
    public ICompraRepository Compras { get; }
    public IDetalleCompraRepository DetallesCompra { get; }
    public ILogInventarioRepository LogsInventario { get; }
    public IPagoRepository Pagos { get; }
    public IGarantiaRepository Garantias { get; }
    public IHistorialKilometrajeRepository HistorialesKilometraje { get; }
    public IOrdenTipoServicioRepository OrdenesTiposServicio { get; }
    public IOrdenMecanicoRepository OrdenesMecanicos { get; }
    public IDetalleOrdenRepository DetallesOrden { get; }
    public ITareaMecanicoRepository TareasMecanicos { get; }
    public INotaOrdenRepository NotasOrden { get; }
    public IHistorialEstadoOrdenRepository HistorialEstadosOrden { get; }
    public IAuditoriaRepository Auditorias { get; }

    public UnitOfWork(
        AutoTallerDbContext context,
        IClienteRepository clientes,
        IVehiculoRepository vehiculos,
        IOrdenServicioRepository ordenesServicio,
        IRepuestoRepository repuestos,
        IFacturaRepository facturas,
        IUsuarioRepository usuarios,
        IRolRepository roles,
        IEstadoOrdenRepository estadosOrden,
        IEstadoFacturaRepository estadosFactura,
        IMetodoPagoRepository metodosPago,
        IUnidadMedidaRepository unidadesMedida,
        ICategoriaRepuestoRepository categoriasRepuesto,
        ITipoServicioRepository tiposServicio,
        IPaisRepository paises,
        IDepartamentoRepository departamentos,
        ICiudadRepository ciudades,
        IMarcaVehiculoRepository marcasVehiculo,
        IModeloVehiculoRepository modelosVehiculo,
        IClienteCorreoRepository clienteCorreos,
        IClienteTelefonoRepository clienteTelefonos,
        IClienteDireccionRepository clienteDirecciones,
        ICitaRepository citas,
        IProveedorRepository proveedores,
        IRepuestoProveedorRepository repuestosProveedor,
        ICompraRepository compras,
        IDetalleCompraRepository detallesCompra,
        ILogInventarioRepository logsInventario,
        IPagoRepository pagos,
        IGarantiaRepository garantias,
        IHistorialKilometrajeRepository historialesKilometraje,
        IOrdenTipoServicioRepository ordenesTiposServicio,
        IOrdenMecanicoRepository ordenesMecanicos,
        IDetalleOrdenRepository detallesOrden,
        ITareaMecanicoRepository tareasMecanicos,
        INotaOrdenRepository notasOrden,
        IHistorialEstadoOrdenRepository historialEstadosOrden,
        IAuditoriaRepository auditorias)
    {
        _context = context;
        Clientes = clientes;
        Vehiculos = vehiculos;
        OrdenesServicio = ordenesServicio;
        Repuestos = repuestos;
        Facturas = facturas;
        Usuarios = usuarios;
        Roles = roles;
        EstadosOrden = estadosOrden;
        EstadosFactura = estadosFactura;
        MetodosPago = metodosPago;
        UnidadesMedida = unidadesMedida;
        CategoriasRepuesto = categoriasRepuesto;
        TiposServicio = tiposServicio;
        Paises = paises;
        Departamentos = departamentos;
        Ciudades = ciudades;
        MarcasVehiculo = marcasVehiculo;
        ModelosVehiculo = modelosVehiculo;
        ClienteCorreos = clienteCorreos;
        ClienteTelefonos = clienteTelefonos;
        ClienteDirecciones = clienteDirecciones;
        Citas = citas;
        Proveedores = proveedores;
        RepuestosProveedor = repuestosProveedor;
        Compras = compras;
        DetallesCompra = detallesCompra;
        LogsInventario = logsInventario;
        Pagos = pagos;
        Garantias = garantias;
        HistorialesKilometraje = historialesKilometraje;
        OrdenesTiposServicio = ordenesTiposServicio;
        OrdenesMecanicos = ordenesMecanicos;
        DetallesOrden = detallesOrden;
        TareasMecanicos = tareasMecanicos;
        NotasOrden = notasOrden;
        HistorialEstadosOrden = historialEstadosOrden;
        Auditorias = auditorias;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        var executionStrategy = _context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                await operation(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}

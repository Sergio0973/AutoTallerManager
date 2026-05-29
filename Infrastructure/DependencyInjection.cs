using Application.Abstractions;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AutoTallerDbContext>(options =>
        {
            string connectionString = configuration.GetConnectionString("Postgres")!;
            options.UseNpgsql(connectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVehiculoRepository, VehiculoRepository>();
        services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
        services.AddScoped<IRepuestoRepository, RepuestoRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IEstadoOrdenRepository, EstadoOrdenRepository>();
        services.AddScoped<IEstadoFacturaRepository, EstadoFacturaRepository>();
        services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
        services.AddScoped<IUnidadMedidaRepository, UnidadMedidaRepository>();
        services.AddScoped<ICategoriaRepuestoRepository, CategoriaRepuestoRepository>();
        services.AddScoped<ITipoServicioRepository, TipoServicioRepository>();
        services.AddScoped<IPaisRepository, PaisRepository>();
        services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
        services.AddScoped<ICiudadRepository, CiudadRepository>();
        services.AddScoped<IMarcaVehiculoRepository, MarcaVehiculoRepository>();
        services.AddScoped<IModeloVehiculoRepository, ModeloVehiculoRepository>();
        services.AddScoped<IClienteCorreoRepository, ClienteCorreoRepository>();
        services.AddScoped<IClienteTelefonoRepository, ClienteTelefonoRepository>();
        services.AddScoped<IClienteDireccionRepository, ClienteDireccionRepository>();
        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IRepuestoProveedorRepository, RepuestoProveedorRepository>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IDetalleCompraRepository, DetalleCompraRepository>();
        services.AddScoped<ILogInventarioRepository, LogInventarioRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IGarantiaRepository, GarantiaRepository>();
        services.AddScoped<IHistorialKilometrajeRepository, HistorialKilometrajeRepository>();
        services.AddScoped<IOrdenTipoServicioRepository, OrdenTipoServicioRepository>();
        services.AddScoped<IOrdenMecanicoRepository, OrdenMecanicoRepository>();
        services.AddScoped<IDetalleOrdenRepository, DetalleOrdenRepository>();
        services.AddScoped<ITareaMecanicoRepository, TareaMecanicoRepository>();
        services.AddScoped<INotaOrdenRepository, NotaOrdenRepository>();
        services.AddScoped<IHistorialEstadoOrdenRepository, HistorialEstadoOrdenRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

        return services;
    }
}

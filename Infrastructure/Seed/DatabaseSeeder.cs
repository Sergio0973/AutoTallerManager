using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.CategoriaRepuestos;
using Domain.ValueObjects.EstadoFacturas;
using Domain.ValueObjects.EstadoOrdenes;
using Domain.ValueObjects.MetodoPagos;
using Domain.ValueObjects.Roles;
using Domain.ValueObjects.TipoServicios;
using Domain.ValueObjects.UnidadMedidas;
using Domain.ValueObjects.Usuarios;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(this IServiceProvider services, bool seedDevelopmentUsers)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await SeedRolesAsync(context);
        await SeedEstadosOrdenAsync(context);
        await SeedEstadosFacturaAsync(context);
        await SeedMetodosPagoAsync(context);
        await SeedTiposServicioAsync(context);
        await SeedInventarioBaseAsync(context);

        if (seedDevelopmentUsers)
        {
            await SeedUsuariosDesarrolloAsync(context, passwordHasher);
        }
    }

    private static async Task SeedRolesAsync(AutoTallerDbContext context)
    {
        var roles = await context.Roles.ToListAsync();

        await AddRolIfMissingAsync(context, roles, "Admin", "Acceso total al sistema.");
        await AddRolIfMissingAsync(context, roles, "Mecanico", "Usuario encargado de ejecutar trabajos de taller.");
        await AddRolIfMissingAsync(context, roles, "Recepcionista", "Usuario encargado de registrar clientes, vehiculos, citas y ordenes.");

        await context.SaveChangesAsync();
    }

    private static async Task SeedEstadosOrdenAsync(AutoTallerDbContext context)
    {
        var estados = await context.EstadosOrden.ToListAsync();

        await AddEstadoOrdenIfMissingAsync(context, estados, "Pendiente", "Orden registrada pendiente por iniciar.");
        await AddEstadoOrdenIfMissingAsync(context, estados, "En proceso", "Orden actualmente en ejecucion.");
        await AddEstadoOrdenIfMissingAsync(context, estados, "Completada", "Orden finalizada y disponible para facturacion.");
        await AddEstadoOrdenIfMissingAsync(context, estados, "Cancelada", "Orden cancelada y sin posibilidad de facturacion.");

        await context.SaveChangesAsync();
    }

    private static async Task SeedEstadosFacturaAsync(AutoTallerDbContext context)
    {
        var estados = await context.EstadosFactura.ToListAsync();

        await AddEstadoFacturaIfMissingAsync(context, estados, "Emitida");
        await AddEstadoFacturaIfMissingAsync(context, estados, "Pagada");
        await AddEstadoFacturaIfMissingAsync(context, estados, "Anulada");

        await context.SaveChangesAsync();
    }

    private static async Task SeedMetodosPagoAsync(AutoTallerDbContext context)
    {
        var metodos = await context.MetodosPago.ToListAsync();

        await AddMetodoPagoIfMissingAsync(context, metodos, "Efectivo", "Pago realizado en efectivo.");
        await AddMetodoPagoIfMissingAsync(context, metodos, "Tarjeta", "Pago realizado con tarjeta.");
        await AddMetodoPagoIfMissingAsync(context, metodos, "Transferencia", "Pago realizado por transferencia bancaria.");

        await context.SaveChangesAsync();
    }

    private static async Task SeedTiposServicioAsync(AutoTallerDbContext context)
    {
        var tipos = await context.TiposServicio.ToListAsync();

        await AddTipoServicioIfMissingAsync(context, tipos, "Diagnostico", "Revision inicial para identificar fallas.", 1);
        await AddTipoServicioIfMissingAsync(context, tipos, "Mantenimiento preventivo", "Servicio programado para prevenir fallas.", 1);
        await AddTipoServicioIfMissingAsync(context, tipos, "Reparacion", "Correccion de fallas detectadas en el vehiculo.", 2);

        await context.SaveChangesAsync();
    }

    private static async Task SeedInventarioBaseAsync(AutoTallerDbContext context)
    {
        var categorias = await context.CategoriasRepuesto.ToListAsync();
        var unidades = await context.UnidadesMedida.ToListAsync();

        await AddCategoriaIfMissingAsync(context, categorias, "Encendido", "Componentes del sistema de encendido.");
        await AddCategoriaIfMissingAsync(context, categorias, "Frenos", "Componentes del sistema de frenos.");
        await AddCategoriaIfMissingAsync(context, categorias, "Filtros", "Filtros y elementos de mantenimiento.");

        await AddUnidadIfMissingAsync(context, unidades, "Unidad", "und");
        await AddUnidadIfMissingAsync(context, unidades, "Litro", "L");

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsuariosDesarrolloAsync(AutoTallerDbContext context, IPasswordHasher passwordHasher)
    {
        var roles = await context.Roles.ToListAsync();
        var usuarios = await context.Usuarios.ToListAsync();

        var adminId = GetRolId(roles, "Admin");
        var mecanicoId = GetRolId(roles, "Mecanico");
        var recepcionistaId = GetRolId(roles, "Recepcionista");

        await AddUsuarioIfMissingAsync(context, usuarios, adminId, "admin.seed@autotaller.com", "Admin Seed", "Admin123!", passwordHasher);
        await AddUsuarioIfMissingAsync(context, usuarios, mecanicoId, "mecanico.seed@autotaller.com", "Mecanico Seed", "Mecanico123!", passwordHasher);
        await AddUsuarioIfMissingAsync(context, usuarios, recepcionistaId, "recepcionista.seed@autotaller.com", "Recepcionista Seed", "Recepcionista123!", passwordHasher);

        await context.SaveChangesAsync();
    }

    private static Task AddRolIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<Rol> roles, string nombre, string descripcion)
    {
        if (roles.Any(r => Same(r.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.Roles.Add(new Rol(NombreRol.Create(nombre), DescripcionRol.Create(descripcion)));
        return Task.CompletedTask;
    }

    private static Task AddEstadoOrdenIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<EstadoOrden> estados, string nombre, string descripcion)
    {
        if (estados.Any(e => Same(e.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.EstadosOrden.Add(new EstadoOrden(NombreEstado.Create(nombre), DescripcionEstado.Create(descripcion)));
        return Task.CompletedTask;
    }

    private static Task AddEstadoFacturaIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<EstadoFactura> estados, string nombre)
    {
        if (estados.Any(e => Same(e.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.EstadosFactura.Add(new EstadoFactura(NombreEstadoFactura.Create(nombre)));
        return Task.CompletedTask;
    }

    private static Task AddMetodoPagoIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<MetodoPago> metodos, string nombre, string descripcion)
    {
        if (metodos.Any(m => Same(m.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.MetodosPago.Add(new MetodoPago(NombreMetodoPago.Create(nombre), DescripcionMetodoPago.Create(descripcion)));
        return Task.CompletedTask;
    }

    private static Task AddTipoServicioIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<TipoServicio> tipos, string nombre, string descripcion, int diasEstimados)
    {
        if (tipos.Any(t => Same(t.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.TiposServicio.Add(new TipoServicio(
            NombreTipoServicio.Create(nombre),
            DescripcionTipoServicio.Create(descripcion),
            DiasEstimados.Create(diasEstimados)));

        return Task.CompletedTask;
    }

    private static Task AddCategoriaIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<CategoriaRepuesto> categorias, string nombre, string descripcion)
    {
        if (categorias.Any(c => Same(c.Nombre.Value, nombre)))
        {
            return Task.CompletedTask;
        }

        context.CategoriasRepuesto.Add(new CategoriaRepuesto(NombreCategoria.Create(nombre), DescripcionCategoria.Create(descripcion)));
        return Task.CompletedTask;
    }

    private static Task AddUnidadIfMissingAsync(AutoTallerDbContext context, IReadOnlyCollection<UnidadMedida> unidades, string nombre, string abreviatura)
    {
        if (unidades.Any(u => Same(u.Nombre.Value, nombre) || Same(u.Abreviatura.Value, abreviatura)))
        {
            return Task.CompletedTask;
        }

        context.UnidadesMedida.Add(new UnidadMedida(NombreUnidad.Create(nombre), Abreviatura.Create(abreviatura)));
        return Task.CompletedTask;
    }

    private static Task AddUsuarioIfMissingAsync(
        AutoTallerDbContext context,
        IReadOnlyCollection<Usuario> usuarios,
        int rolId,
        string correo,
        string nombre,
        string password,
        IPasswordHasher passwordHasher)
    {
        if (usuarios.Any(u => Same(u.Correo.Value, correo)))
        {
            return Task.CompletedTask;
        }

        context.Usuarios.Add(new Usuario(
            rolId,
            CorreoUsuario.Create(correo),
            NombreUsuario.Create(nombre),
            passwordHasher.Hash(password)));

        return Task.CompletedTask;
    }

    private static int GetRolId(IReadOnlyCollection<Rol> roles, string nombre)
    {
        return roles.First(r => Same(r.Nombre.Value, nombre)).Id;
    }

    private static bool Same(string left, string right)
    {
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
    }
}

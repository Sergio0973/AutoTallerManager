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

    public UnitOfWork(
        AutoTallerDbContext context,
        IClienteRepository clientes,
        IVehiculoRepository vehiculos,
        IOrdenServicioRepository ordenesServicio,
        IRepuestoRepository repuestos,
        IFacturaRepository facturas,
        IUsuarioRepository usuarios)
    {
        _context = context;
        Clientes = clientes;
        Vehiculos = vehiculos;
        OrdenesServicio = ordenesServicio;
        Repuestos = repuestos;
        Facturas = facturas;
        Usuarios = usuarios;
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

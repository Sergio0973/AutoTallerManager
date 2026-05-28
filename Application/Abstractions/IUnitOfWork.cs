namespace Application.Abstractions;

public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    IVehiculoRepository Vehiculos { get; }
    IOrdenServicioRepository OrdenesServicio { get; }
    IRepuestoRepository Repuestos { get; }
    IFacturaRepository Facturas { get; }
    IUsuarioRepository Usuarios { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}

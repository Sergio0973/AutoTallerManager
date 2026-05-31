using Application.Abstractions;
using Domain.Entities;

namespace Application.Common.Orders;

public static class OrdenEstadoGuard
{
    private static readonly string[] EstadosTerminales = ["Completada", "Cancelada"];

    public static async Task EnsureEditableAsync(IUnitOfWork uow, OrdenServicio orden, CancellationToken ct)
    {
        var estado = await uow.EstadosOrden.GetByIdAsync(orden.EstadoId, ct)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        if (EstadosTerminales.Any(e => string.Equals(e, estado.Nombre.Value, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("No se puede modificar una orden completada o cancelada.");
        }
    }

    public static async Task EnsureCanBeFacturadaAsync(IUnitOfWork uow, OrdenServicio orden, CancellationToken ct)
    {
        var estado = await uow.EstadosOrden.GetByIdAsync(orden.EstadoId, ct)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        if (string.Equals(estado.Nombre.Value, "Cancelada", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("No se puede facturar una orden cancelada.");
        }
    }
}

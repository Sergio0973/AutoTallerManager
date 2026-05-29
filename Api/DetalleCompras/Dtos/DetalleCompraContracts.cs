namespace Api.DetalleCompras.Dtos;

public sealed record DetalleCompraDto(
    int Id,
    int CompraId,
    int RepuestoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record CreateDetalleCompraRequest(int CompraId, int RepuestoId, int Cantidad, decimal PrecioUnitario);

public sealed record UpdateDetalleCompraRequest(int Cantidad, decimal PrecioUnitario);

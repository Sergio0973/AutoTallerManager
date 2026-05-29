namespace Api.DetalleOrdenes.Dtos;

public sealed record DetalleOrdenDto(
    int Id,
    int OrdenId,
    int RepuestoId,
    int Cantidad,
    decimal PrecioSnapshot,
    decimal Subtotal);

public sealed record CreateDetalleOrdenRequest(
    int OrdenId,
    int RepuestoId,
    int UsuarioId,
    int Cantidad,
    decimal PrecioSnapshot);

public sealed record UpdateDetalleOrdenRequest(
    int UsuarioId,
    int Cantidad,
    decimal PrecioSnapshot);

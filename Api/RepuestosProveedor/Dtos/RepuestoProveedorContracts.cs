namespace Api.RepuestosProveedor.Dtos;

public sealed record RepuestoProveedorDto(
    int Id,
    int RepuestoId,
    int ProveedorId,
    decimal PrecioCompra,
    bool Principal);

public sealed record CreateRepuestoProveedorRequest(
    int RepuestoId,
    int ProveedorId,
    decimal PrecioCompra,
    bool Principal);

public sealed record UpdateRepuestoProveedorRequest(
    decimal PrecioCompra,
    bool Principal);

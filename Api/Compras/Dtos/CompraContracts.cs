namespace Api.Compras.Dtos;

public sealed record CompraDto(
    int Id,
    int ProveedorId,
    int UsuarioId,
    DateOnly FechaCompra,
    decimal Total,
    string Estado,
    string? Observaciones);

public sealed record CreateCompraRequest(
    int ProveedorId,
    int UsuarioId,
    DateOnly FechaCompra,
    string Estado,
    string? Observaciones);

public sealed record UpdateCompraRequest(
    int ProveedorId,
    int UsuarioId,
    DateOnly FechaCompra,
    string Estado,
    string? Observaciones);

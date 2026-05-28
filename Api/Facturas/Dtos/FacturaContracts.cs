namespace Api.Facturas.Dtos;

public sealed record FacturaDto(
    int Id,
    int OrdenId,
    int EstadoFacturaId,
    int UsuarioId,
    decimal ManoDeObra,
    decimal CostoRepuestos,
    decimal Descuento,
    decimal ImpuestoPct,
    decimal Subtotal,
    decimal Total,
    DateOnly FechaEmision,
    string? Observaciones);

public sealed record CreateFacturaRequest(
    int OrdenId,
    int EstadoFacturaId,
    int UsuarioId,
    decimal ManoDeObra,
    decimal CostoRepuestos,
    decimal Descuento,
    decimal ImpuestoPct,
    decimal Subtotal,
    decimal Total,
    DateOnly FechaEmision,
    string? Observaciones);

public sealed record UpdateFacturaRequest(
    int EstadoFacturaId,
    decimal ManoDeObra,
    decimal CostoRepuestos,
    decimal Descuento,
    decimal ImpuestoPct,
    decimal Subtotal,
    decimal Total,
    string? Observaciones);

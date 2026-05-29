namespace Api.Pagos.Dtos;

public sealed record PagoDto(
    int Id,
    int FacturaId,
    int MetodoPagoId,
    decimal Monto,
    DateTime FechaPago,
    string? Referencia,
    string Estado);

public sealed record CreatePagoRequest(
    int FacturaId,
    int MetodoPagoId,
    decimal Monto,
    string? Referencia,
    string Estado);

public sealed record UpdatePagoRequest(string Estado);

namespace Api.LogsInventario.Dtos;

public sealed record LogInventarioDto(
    long Id,
    int RepuestoId,
    int UsuarioId,
    int? OrdenId,
    int? CompraId,
    string TipoMovimiento,
    int Cantidad,
    int StockResultante,
    DateTime Fecha,
    string? Motivo);

namespace Api.HistorialesEstadoOrden.Dtos;

public sealed record HistorialEstadoOrdenDto(
    int Id,
    int OrdenId,
    int EstadoId,
    int UsuarioId,
    DateTime FechaCambio,
    string? Observacion);

public sealed record CreateHistorialEstadoOrdenRequest(
    int OrdenId,
    int EstadoId,
    int UsuarioId,
    string? Observacion);

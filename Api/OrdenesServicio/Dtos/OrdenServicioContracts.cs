namespace Api.OrdenesServicio.Dtos;

public sealed record OrdenServicioDto(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly FechaIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones);

public sealed record CreateOrdenServicioRequest(
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly FechaIngreso,
    DateOnly? FechaEstimada,
    string? Observaciones);

public sealed record UpdateOrdenServicioRequest(
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones);

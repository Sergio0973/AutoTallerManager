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
    string? Vin,
    int RecepcionistaId,
    int? EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateTime FechaIngreso,
    DateOnly? FechaEstimada,
    string? TipoServicio,
    int? TipoServicioId,
    int? MecanicoId,
    IReadOnlyList<CreateOrdenServicioRepuestoRequest>? Repuestos,
    string? Observaciones);

public sealed record CreateOrdenServicioRepuestoRequest(int RepuestoId, int Cantidad);

public sealed record CreateOrdenServicioResponse(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly FechaIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones,
    int? MecanicoId,
    int? TipoServicioId,
    string EstadoInicial,
    IReadOnlyList<RepuestoReservadoResponse> RepuestosReservados);

public sealed record RepuestoReservadoResponse(int RepuestoId, int Cantidad, decimal PrecioSnapshot, decimal Subtotal);

public sealed record UpdateOrdenServicioRequest(
    int VehiculoId,
    int RecepcionistaId,
    int EstadoId,
    int? CitaId,
    int KilometrajeIngreso,
    DateOnly? FechaEstimada,
    DateOnly? FechaEntregaReal,
    string? Observaciones);

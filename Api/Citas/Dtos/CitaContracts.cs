namespace Api.Citas.Dtos;

public sealed record CitaDto(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int TipoServicioId,
    DateOnly FechaCita,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Estado,
    string? Observaciones);

public sealed record CreateCitaRequest(
    int VehiculoId,
    int RecepcionistaId,
    int TipoServicioId,
    DateOnly FechaCita,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Estado,
    string? Observaciones);

public sealed record UpdateCitaRequest(
    int VehiculoId,
    int RecepcionistaId,
    int TipoServicioId,
    DateOnly FechaCita,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Estado,
    string? Observaciones);

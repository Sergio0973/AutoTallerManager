namespace Api.HistorialesKilometraje.Dtos;

public sealed record HistorialKilometrajeDto(
    int Id,
    int VehiculoId,
    int Kilometraje,
    DateOnly Fecha,
    string Fuente);

public sealed record CreateHistorialKilometrajeRequest(int VehiculoId, int Kilometraje, DateOnly Fecha, string Fuente);

public sealed record UpdateHistorialKilometrajeRequest(int VehiculoId, int Kilometraje, DateOnly Fecha, string Fuente);

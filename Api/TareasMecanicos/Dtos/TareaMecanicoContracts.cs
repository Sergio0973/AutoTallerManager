namespace Api.TareasMecanicos.Dtos;

public sealed record TareaMecanicoDto(
    int Id,
    int OrdenId,
    int MecanicoId,
    int TipoServicioId,
    string Descripcion,
    decimal HorasTrabajadas,
    decimal CostoHora,
    decimal CostoTotal,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin);

public sealed record CreateTareaMecanicoRequest(
    int OrdenId,
    int MecanicoId,
    int TipoServicioId,
    string Descripcion,
    decimal HorasTrabajadas,
    decimal CostoHora,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin);

public sealed record UpdateTareaMecanicoRequest(
    string Descripcion,
    decimal HorasTrabajadas,
    decimal CostoHora,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin);

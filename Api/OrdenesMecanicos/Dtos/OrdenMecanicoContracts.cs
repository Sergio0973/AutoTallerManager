namespace Api.OrdenesMecanicos.Dtos;

public sealed record OrdenMecanicoDto(int Id, int OrdenId, int MecanicoId, DateOnly FechaAsignacion);

public sealed record CreateOrdenMecanicoRequest(int OrdenId, int MecanicoId, DateOnly FechaAsignacion);

public sealed record UpdateOrdenMecanicoRequest(DateOnly FechaAsignacion);

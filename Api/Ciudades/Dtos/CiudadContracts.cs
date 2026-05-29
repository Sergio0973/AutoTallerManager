namespace Api.Ciudades.Dtos;

public sealed record CiudadDto(int Id, int DepartamentoId, string Nombre);

public sealed record CreateCiudadRequest(int DepartamentoId, string Nombre);

public sealed record UpdateCiudadRequest(int DepartamentoId, string Nombre);

namespace Api.ClienteDirecciones.Dtos;

public sealed record ClienteDireccionDto(int Id, int ClienteId, int CiudadId, string Direccion, bool Principal);

public sealed record CreateClienteDireccionRequest(int ClienteId, int CiudadId, string Direccion, bool Principal);

public sealed record UpdateClienteDireccionRequest(int ClienteId, int CiudadId, string Direccion, bool Principal);

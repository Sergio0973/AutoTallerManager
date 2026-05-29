namespace Api.ClienteCorreos.Dtos;

public sealed record ClienteCorreoDto(int Id, int ClienteId, string Correo, bool Principal);

public sealed record CreateClienteCorreoRequest(int ClienteId, string Correo, bool Principal);

public sealed record UpdateClienteCorreoRequest(int ClienteId, string Correo, bool Principal);

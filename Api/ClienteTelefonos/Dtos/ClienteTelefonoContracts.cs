namespace Api.ClienteTelefonos.Dtos;

public sealed record ClienteTelefonoDto(int Id, int ClienteId, string Telefono, string Tipo);

public sealed record CreateClienteTelefonoRequest(int ClienteId, string Telefono, string Tipo);

public sealed record UpdateClienteTelefonoRequest(int ClienteId, string Telefono, string Tipo);

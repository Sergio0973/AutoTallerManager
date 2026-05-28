namespace Api.Clientes.Dtos;

public sealed record ClienteDto(int Id, string Nombres, string Apellidos, string Documento, DateTime FechaRegistro, bool Activo);

public sealed record CreateClienteRequest(string Nombres, string Apellidos, string Documento);

public sealed record UpdateClienteRequest(string Nombres, string Apellidos, string Documento);

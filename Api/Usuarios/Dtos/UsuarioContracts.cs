namespace Api.Usuarios.Dtos;

public sealed record UsuarioDto(int Id, int RolId, string Correo, string Nombre, bool Activo, DateTime FechaCreacion);

public sealed record CreateUsuarioRequest(int RolId, string Correo, string Nombre, string Contrasena);

public sealed record UpdateUsuarioRequest(int RolId, string Correo, string Nombre);

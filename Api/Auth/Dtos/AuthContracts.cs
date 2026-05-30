namespace Api.Auth.Dtos;

public sealed record LoginRequest(string Correo, string Contrasena);

public sealed record LoginResponse(
    string Token,
    DateTime ExpiraEn,
    UsuarioAutenticadoDto Usuario);

public sealed record UsuarioAutenticadoDto(
    int Id,
    string Correo,
    string Nombre,
    string Rol);

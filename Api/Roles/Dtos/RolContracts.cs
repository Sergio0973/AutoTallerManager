namespace Api.Roles.Dtos;

public sealed record RolDto(int Id, string Nombre, string Descripcion);

public sealed record CreateRolRequest(string Nombre, string Descripcion);

public sealed record UpdateRolRequest(string Nombre, string Descripcion);

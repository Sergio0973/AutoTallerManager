namespace Api.Paises.Dtos;

public sealed record PaisDto(int Id, string Nombre, string Codigo);

public sealed record CreatePaisRequest(string Nombre, string Codigo);

public sealed record UpdatePaisRequest(string Nombre, string Codigo);

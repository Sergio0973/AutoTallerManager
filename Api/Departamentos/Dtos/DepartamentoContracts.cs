namespace Api.Departamentos.Dtos;

public sealed record DepartamentoDto(int Id, int PaisId, string Nombre);

public sealed record CreateDepartamentoRequest(int PaisId, string Nombre);

public sealed record UpdateDepartamentoRequest(int PaisId, string Nombre);

namespace Api.UnidadesMedida.Dtos;

public sealed record UnidadMedidaDto(int Id, string Nombre, string Abreviatura);

public sealed record CreateUnidadMedidaRequest(string Nombre, string Abreviatura);

public sealed record UpdateUnidadMedidaRequest(string Nombre, string Abreviatura);

namespace Api.TiposServicio.Dtos;

public sealed record TipoServicioDto(int Id, string Nombre, string Descripcion, int DiasEstimados);

public sealed record CreateTipoServicioRequest(string Nombre, string Descripcion, int DiasEstimados);

public sealed record UpdateTipoServicioRequest(string Nombre, string Descripcion, int DiasEstimados);

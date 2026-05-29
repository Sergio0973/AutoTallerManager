namespace Api.EstadosOrden.Dtos;

public sealed record EstadoOrdenDto(int Id, string Nombre, string Descripcion);

public sealed record CreateEstadoOrdenRequest(string Nombre, string Descripcion);

public sealed record UpdateEstadoOrdenRequest(string Nombre, string Descripcion);

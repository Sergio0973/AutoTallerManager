namespace Api.MetodosPago.Dtos;

public sealed record MetodoPagoDto(int Id, string Nombre, string Descripcion);

public sealed record CreateMetodoPagoRequest(string Nombre, string Descripcion);

public sealed record UpdateMetodoPagoRequest(string Nombre, string Descripcion);

namespace Api.EstadosFactura.Dtos;

public sealed record EstadoFacturaDto(int Id, string Nombre);

public sealed record CreateEstadoFacturaRequest(string Nombre);

public sealed record UpdateEstadoFacturaRequest(string Nombre);

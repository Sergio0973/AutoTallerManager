namespace Api.MarcasVehiculo.Dtos;

public sealed record MarcaVehiculoDto(int Id, string Nombre);

public sealed record CreateMarcaVehiculoRequest(string Nombre);

public sealed record UpdateMarcaVehiculoRequest(string Nombre);

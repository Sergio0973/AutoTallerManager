namespace Api.Vehiculos.Dtos;

public sealed record VehiculoDto(int Id, int ClienteId, int ModeloId, string Vin, short Anio, string Placa, string Color, bool Activo);

public sealed record CreateVehiculoRequest(int ClienteId, int ModeloId, string Vin, short Anio, string Placa, string Color);

public sealed record UpdateVehiculoRequest(int ClienteId, int ModeloId, string Vin, short Anio, string Placa, string Color);

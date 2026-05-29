namespace Api.ModelosVehiculo.Dtos;

public sealed record ModeloVehiculoDto(int Id, int MarcaId, string Nombre, short AnioDesde, short AnioHasta);

public sealed record CreateModeloVehiculoRequest(int MarcaId, string Nombre, short AnioDesde, short AnioHasta);

public sealed record UpdateModeloVehiculoRequest(int MarcaId, string Nombre, short AnioDesde, short AnioHasta);

namespace Api.Repuestos.Dtos;

public sealed record RepuestoDto(
    int Id,
    int CategoriaId,
    int UnidadId,
    string Codigo,
    string Descripcion,
    int StockActual,
    int StockMinimo,
    decimal PrecioUnitario,
    bool Activo);

public sealed record CreateRepuestoRequest(
    int CategoriaId,
    int UnidadId,
    string Codigo,
    string Descripcion,
    int StockActual,
    int StockMinimo,
    decimal PrecioUnitario);

public sealed record UpdateRepuestoRequest(
    int CategoriaId,
    int UnidadId,
    string Codigo,
    string Descripcion,
    int StockActual,
    int StockMinimo,
    decimal PrecioUnitario);

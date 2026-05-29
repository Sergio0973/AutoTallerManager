namespace Api.Garantias.Dtos;

public sealed record GarantiaDto(
    int Id,
    int OrdenId,
    int TipoServicioId,
    int MecanicoId,
    DateOnly FechaInicio,
    DateOnly FechaVencimiento,
    string Condiciones,
    string Estado);

public sealed record CreateGarantiaRequest(
    int OrdenId,
    int TipoServicioId,
    int MecanicoId,
    DateOnly FechaInicio,
    DateOnly FechaVencimiento,
    string Condiciones,
    string Estado);

public sealed record UpdateGarantiaRequest(string Estado);

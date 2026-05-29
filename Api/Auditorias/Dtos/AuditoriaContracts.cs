namespace Api.Auditorias.Dtos;

public sealed record AuditoriaDto(
    long Id,
    int UsuarioId,
    string Entidad,
    int EntidadId,
    string TipoAccion,
    string? DatosAnteriores,
    string? DatosNuevos,
    string IpOrigen,
    DateTime Fecha);

public sealed record CreateAuditoriaRequest(
    int UsuarioId,
    string Entidad,
    int EntidadId,
    string TipoAccion,
    string? DatosAnteriores,
    string? DatosNuevos,
    string IpOrigen);

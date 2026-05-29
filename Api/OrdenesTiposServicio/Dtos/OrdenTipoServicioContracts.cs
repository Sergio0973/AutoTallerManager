namespace Api.OrdenesTiposServicio.Dtos;

public sealed record OrdenTipoServicioDto(int Id, int OrdenId, int TipoServicioId);

public sealed record CreateOrdenTipoServicioRequest(int OrdenId, int TipoServicioId);

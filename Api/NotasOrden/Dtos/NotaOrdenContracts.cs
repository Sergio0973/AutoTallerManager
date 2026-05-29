namespace Api.NotasOrden.Dtos;

public sealed record NotaOrdenDto(
    int Id,
    int OrdenId,
    int UsuarioId,
    string Contenido,
    DateTime FechaNota);

public sealed record CreateNotaOrdenRequest(int OrdenId, int UsuarioId, string Contenido);

public sealed record UpdateNotaOrdenRequest(string Contenido);

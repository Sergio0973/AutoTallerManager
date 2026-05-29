namespace Api.CategoriasRepuesto.Dtos;

public sealed record CategoriaRepuestoDto(int Id, string Nombre, string Descripcion);

public sealed record CreateCategoriaRepuestoRequest(string Nombre, string Descripcion);

public sealed record UpdateCategoriaRepuestoRequest(string Nombre, string Descripcion);

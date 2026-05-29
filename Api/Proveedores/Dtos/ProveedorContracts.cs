namespace Api.Proveedores.Dtos;

public sealed record ProveedorDto(
    int Id,
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    int CiudadId,
    bool Activo);

public sealed record CreateProveedorRequest(
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    int CiudadId);

public sealed record UpdateProveedorRequest(
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    int CiudadId,
    bool Activo);

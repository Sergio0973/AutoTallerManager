namespace Application.Abstractions;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int usuarioId,
        string entidad,
        int entidadId,
        string tipoAccion,
        object? datosAnteriores,
        object? datosNuevos,
        CancellationToken ct = default);
}

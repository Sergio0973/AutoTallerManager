using System.Text.Json;
using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Auditorias;

namespace Application.Common.Auditing;

public sealed class AuditoriaService : IAuditoriaService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IUnitOfWork _uow;

    public AuditoriaService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task RegistrarAsync(
        int usuarioId,
        string entidad,
        int entidadId,
        string tipoAccion,
        object? datosAnteriores,
        object? datosNuevos,
        CancellationToken ct = default)
    {
        var auditoria = new Auditoria(
            usuarioId,
            EntidadAuditada.Create(entidad),
            entidadId,
            TipoAccion.Create(tipoAccion),
            ToDatosJson(datosAnteriores),
            ToDatosJson(datosNuevos),
            IpOrigen.Create("SYSTEM"));

        await _uow.Auditorias.AddAsync(auditoria, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static DatosJson? ToDatosJson(object? value)
    {
        return value is null
            ? null
            : DatosJson.Create(JsonSerializer.Serialize(value, JsonOptions));
    }
}

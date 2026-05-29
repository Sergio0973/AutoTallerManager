using Api.Auditorias.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Auditorias.UseCase;
using Domain.ValueObjects.Auditorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Auditorias.Controllers;

public sealed class AuditoriaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public AuditoriaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditoriaDto>>> GetAll(
        [FromQuery] int? usuarioId,
        [FromQuery] string? entidad,
        [FromQuery] string? tipoAccion,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Auditoria> auditorias;

        if (usuarioId.HasValue)
        {
            auditorias = await _uow.Auditorias.GetByUsuarioIdAsync(usuarioId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(entidad))
        {
            auditorias = await _uow.Auditorias.GetByEntidadAsync(EntidadAuditada.Create(entidad), cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(tipoAccion))
        {
            auditorias = await _uow.Auditorias.GetByTipoAccionAsync(TipoAccion.Create(tipoAccion), cancellationToken);
        }
        else if (desde.HasValue && hasta.HasValue)
        {
            auditorias = await _uow.Auditorias.GetByFechaRangeAsync(desde.Value, hasta.Value, cancellationToken);
        }
        else
        {
            auditorias = await _uow.Auditorias.GetAllAsync(cancellationToken);
        }

        return Ok(auditorias.Select(Map).ToList());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AuditoriaDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var auditoria = await _uow.Auditorias.GetByIdAsync(id, cancellationToken);
        return auditoria is null ? NotFound() : Ok(Map(auditoria));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateAuditoria(
                request.UsuarioId,
                request.Entidad,
                request.EntidadId,
                request.TipoAccion,
                request.DatosAnteriores,
                request.DatosNuevos,
                request.IpOrigen),
            cancellationToken);

        var auditoria = await _uow.Auditorias.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(auditoria!));
    }

    private static AuditoriaDto Map(Domain.Entities.Auditoria auditoria) =>
        new(
            auditoria.Id,
            auditoria.UsuarioId,
            auditoria.Entidad.Value,
            auditoria.EntidadId,
            auditoria.TipoAccion.Value,
            auditoria.DatosAnteriores?.Value,
            auditoria.DatosNuevos?.Value,
            auditoria.IpOrigen.Value,
            auditoria.Fecha);
}

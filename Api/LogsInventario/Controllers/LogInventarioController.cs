using Api.Common.Controllers;
using Api.LogsInventario.Dtos;
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.LogsInventario.Controllers;

public sealed class LogInventarioController : BaseApiController
{
    private readonly IUnitOfWork _uow;

    public LogInventarioController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LogInventarioDto>>> GetAll([FromQuery] int? repuestoId, [FromQuery] int? compraId, [FromQuery] int? usuarioId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.LogInventario> logs;

        if (repuestoId.HasValue)
        {
            logs = await _uow.LogsInventario.GetByRepuestoIdAsync(repuestoId.Value, cancellationToken);
        }
        else if (compraId.HasValue)
        {
            logs = await _uow.LogsInventario.GetByCompraIdAsync(compraId.Value, cancellationToken);
        }
        else if (usuarioId.HasValue)
        {
            logs = await _uow.LogsInventario.GetByUsuarioIdAsync(usuarioId.Value, cancellationToken);
        }
        else
        {
            logs = await _uow.LogsInventario.GetAllAsync(cancellationToken);
        }

        return Ok(logs.Select(Map).ToList());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<LogInventarioDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var log = await _uow.LogsInventario.GetByIdAsync(id, cancellationToken);
        return log is null ? NotFound() : Ok(Map(log));
    }

    private static LogInventarioDto Map(Domain.Entities.LogInventario log) =>
        new(
            log.Id,
            log.RepuestoId,
            log.UsuarioId,
            log.OrdenId,
            log.CompraId,
            log.TipoMovimiento.Value,
            log.Cantidad,
            log.StockResultante,
            log.Fecha,
            log.Motivo?.Value);
}

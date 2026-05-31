using Api.Common.Controllers;
using Api.HistorialesEstadoOrden.Dtos;
using Application.Abstractions;
using Application.HistorialesEstadoOrden.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.HistorialesEstadoOrden.Controllers;

[Authorize(Policy = "Mecanico")]
public sealed class HistorialEstadoOrdenController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public HistorialEstadoOrdenController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HistorialEstadoOrdenDto>>> GetAll(
        [FromQuery] int? ordenId,
        [FromQuery] int? estadoId,
        [FromQuery] int? usuarioId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.HistorialEstadoOrden> historiales;

        if (ordenId.HasValue)
        {
            historiales = await _uow.HistorialEstadosOrden.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (estadoId.HasValue)
        {
            historiales = await _uow.HistorialEstadosOrden.GetByEstadoIdAsync(estadoId.Value, cancellationToken);
        }
        else if (usuarioId.HasValue)
        {
            historiales = await _uow.HistorialEstadosOrden.GetByUsuarioIdAsync(usuarioId.Value, cancellationToken);
        }
        else
        {
            historiales = await _uow.HistorialEstadosOrden.GetAllAsync(cancellationToken);
        }

        return Ok(historiales.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HistorialEstadoOrdenDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var historial = await _uow.HistorialEstadosOrden.GetByIdAsync(id, cancellationToken);
        return historial is null ? NotFound() : Ok(Map(historial));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHistorialEstadoOrdenRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateHistorialEstadoOrden(
                request.OrdenId,
                request.EstadoId,
                request.UsuarioId,
                request.Observacion),
            cancellationToken);

        var historial = await _uow.HistorialEstadosOrden.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(historial!));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var historial = await _uow.HistorialEstadosOrden.GetByIdAsync(id, cancellationToken);
        if (historial is null)
        {
            return NotFound();
        }

        await _uow.HistorialEstadosOrden.RemoveAsync(historial, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static HistorialEstadoOrdenDto Map(Domain.Entities.HistorialEstadoOrden historial) =>
        new(
            historial.Id,
            historial.OrdenId,
            historial.EstadoId,
            historial.UsuarioId,
            historial.FechaCambio,
            historial.Observacion?.Value);
}

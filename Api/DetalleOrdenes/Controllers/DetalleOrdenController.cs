using Api.Common.Controllers;
using Api.DetalleOrdenes.Dtos;
using Application.Abstractions;
using Application.DetalleOrdenes.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.DetalleOrdenes.Controllers;

public sealed class DetalleOrdenController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public DetalleOrdenController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DetalleOrdenDto>>> GetAll([FromQuery] int? ordenId, [FromQuery] int? repuestoId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.DetalleOrden> detalles;

        if (ordenId.HasValue)
        {
            detalles = await _uow.DetallesOrden.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (repuestoId.HasValue)
        {
            detalles = await _uow.DetallesOrden.GetByRepuestoIdAsync(repuestoId.Value, cancellationToken);
        }
        else
        {
            detalles = await _uow.DetallesOrden.GetAllAsync(cancellationToken);
        }

        return Ok(detalles.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetalleOrdenDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var detalle = await _uow.DetallesOrden.GetByIdAsync(id, cancellationToken);
        return detalle is null ? NotFound() : Ok(Map(detalle));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDetalleOrdenRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateDetalleOrden(
                request.OrdenId,
                request.RepuestoId,
                request.UsuarioId,
                request.Cantidad,
                request.PrecioSnapshot),
            cancellationToken);

        var detalle = await _uow.DetallesOrden.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(detalle!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDetalleOrdenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateDetalleOrden(id, request.UsuarioId, request.Cantidad, request.PrecioSnapshot), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] int usuarioId, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteDetalleOrden(id, usuarioId), cancellationToken);
        return NoContent();
    }

    private static DetalleOrdenDto Map(Domain.Entities.DetalleOrden detalle) =>
        new(
            detalle.Id,
            detalle.OrdenId,
            detalle.RepuestoId,
            detalle.Cantidad.Value,
            detalle.PrecioSnapshot.Value,
            detalle.Cantidad.Value * detalle.PrecioSnapshot.Value);
}

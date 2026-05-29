using Api.Common.Controllers;
using Api.DetalleCompras.Dtos;
using Application.Abstractions;
using Application.DetalleCompras.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.DetalleCompras.Controllers;

public sealed class DetalleCompraController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public DetalleCompraController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DetalleCompraDto>>> GetAll([FromQuery] int? compraId, [FromQuery] int? repuestoId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.DetalleCompra> detalles;

        if (compraId.HasValue)
        {
            detalles = await _uow.DetallesCompra.GetByCompraIdAsync(compraId.Value, cancellationToken);
        }
        else if (repuestoId.HasValue)
        {
            detalles = await _uow.DetallesCompra.GetByRepuestoIdAsync(repuestoId.Value, cancellationToken);
        }
        else
        {
            detalles = await _uow.DetallesCompra.GetAllAsync(cancellationToken);
        }

        return Ok(detalles.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetalleCompraDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var detalle = await _uow.DetallesCompra.GetByIdAsync(id, cancellationToken);
        return detalle is null ? NotFound() : Ok(Map(detalle));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDetalleCompraRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateDetalleCompra(request.CompraId, request.RepuestoId, request.Cantidad, request.PrecioUnitario), cancellationToken);
        var detalle = await _uow.DetallesCompra.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(detalle!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDetalleCompraRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateDetalleCompra(id, request.Cantidad, request.PrecioUnitario), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteDetalleCompra(id), cancellationToken);
        return NoContent();
    }

    private static DetalleCompraDto Map(Domain.Entities.DetalleCompra detalle) =>
        new(
            detalle.Id,
            detalle.CompraId,
            detalle.RepuestoId,
            detalle.Cantidad.Value,
            detalle.PrecioUnitario.Value,
            detalle.Cantidad.Value * detalle.PrecioUnitario.Value);
}

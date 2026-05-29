using Api.Common.Controllers;
using Api.Pagos.Dtos;
using Application.Abstractions;
using Application.Pagos.UseCase;
using Domain.ValueObjects.Pagos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Pagos.Controllers;

public sealed class PagoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public PagoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PagoDto>>> GetAll(
        [FromQuery] int? facturaId,
        [FromQuery] int? metodoPagoId,
        [FromQuery] string? estado,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Pago> pagos;

        if (facturaId.HasValue)
        {
            pagos = await _uow.Pagos.GetByFacturaIdAsync(facturaId.Value, cancellationToken);
        }
        else if (metodoPagoId.HasValue)
        {
            pagos = await _uow.Pagos.GetByMetodoPagoIdAsync(metodoPagoId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(estado))
        {
            pagos = await _uow.Pagos.GetByEstadoAsync(EstadoPago.Create(estado), cancellationToken);
        }
        else
        {
            pagos = await _uow.Pagos.GetAllAsync(cancellationToken);
        }

        return Ok(pagos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PagoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var pago = await _uow.Pagos.GetByIdAsync(id, cancellationToken);
        return pago is null ? NotFound() : Ok(Map(pago));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePagoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreatePago(
                request.FacturaId,
                request.MetodoPagoId,
                request.Monto,
                request.Referencia,
                request.Estado),
            cancellationToken);

        var pago = await _uow.Pagos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(pago!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePagoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdatePago(id, request.Estado), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var pago = await _uow.Pagos.GetByIdAsync(id, cancellationToken);
        if (pago is null)
        {
            return NotFound();
        }

        await _uow.Pagos.RemoveAsync(pago, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static PagoDto Map(Domain.Entities.Pago pago) =>
        new(
            pago.Id,
            pago.FacturaId,
            pago.MetodoPagoId,
            pago.Monto.Value,
            pago.FechaPago,
            pago.Referencia?.Value,
            pago.Estado.Value);
}

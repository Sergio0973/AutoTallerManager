using Api.Common.Controllers;
using Api.Facturas.Dtos;
using Application.Abstractions;
using Application.Facturas.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Facturas.Controllers;

public sealed class FacturaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public FacturaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FacturaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var facturas = await _uow.Facturas.GetAllAsync(cancellationToken);
        return Ok(facturas.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FacturaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var factura = await _uow.Facturas.GetByIdAsync(id, cancellationToken);
        return factura is null ? NotFound() : Ok(Map(factura));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFacturaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateFactura(
                request.OrdenId,
                request.EstadoFacturaId,
                request.UsuarioId,
                request.ManoDeObra,
                request.CostoRepuestos,
                request.Descuento,
                request.ImpuestoPct,
                request.Subtotal,
                request.Total,
                request.FechaEmision,
                request.Observaciones),
            cancellationToken);

        var factura = await _uow.Facturas.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(factura!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFacturaRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateFactura(
                id,
                request.EstadoFacturaId,
                request.ManoDeObra,
                request.CostoRepuestos,
                request.Descuento,
                request.ImpuestoPct,
                request.Subtotal,
                request.Total,
                request.Observaciones),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var factura = await _uow.Facturas.GetByIdAsync(id, cancellationToken);
        if (factura is null)
        {
            return NotFound();
        }

        await _uow.Facturas.RemoveAsync(factura, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static FacturaDto Map(Domain.Entities.Factura factura) =>
        new(
            factura.Id,
            factura.OrdenId,
            factura.EstadoFacturaId,
            factura.UsuarioId,
            factura.Valores.ManoDeObra,
            factura.Valores.CostoRepuestos,
            factura.Valores.Descuento,
            factura.Valores.ImpuestoPct,
            factura.Valores.Subtotal,
            factura.Valores.Total,
            factura.FechaEmision,
            factura.Observaciones?.Value);
}

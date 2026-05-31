using Api.Common.Controllers;
using Api.EstadosFactura.Dtos;
using Application.Abstractions;
using Application.EstadosFactura.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.EstadosFactura.Controllers;

[Authorize(Policy = "Admin")]
public sealed class EstadoFacturaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public EstadoFacturaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EstadoFacturaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var estados = await _uow.EstadosFactura.GetAllAsync(cancellationToken);
        return Ok(estados.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstadoFacturaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosFactura.GetByIdAsync(id, cancellationToken);
        return estado is null ? NotFound() : Ok(Map(estado));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstadoFacturaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateEstadoFactura(request.Nombre), cancellationToken);
        var estado = await _uow.EstadosFactura.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(estado!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEstadoFacturaRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateEstadoFactura(id, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosFactura.GetByIdAsync(id, cancellationToken);
        if (estado is null)
        {
            return NotFound();
        }

        if (await _uow.EstadosFactura.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el estado de factura porque tiene facturas asociadas." });
        }

        await _uow.EstadosFactura.RemoveAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static EstadoFacturaDto Map(Domain.Entities.EstadoFactura estado) =>
        new(estado.Id, estado.Nombre.Value);
}

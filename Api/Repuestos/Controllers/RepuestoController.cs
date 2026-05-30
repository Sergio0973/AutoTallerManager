using Api.Common.Controllers;
using Api.Repuestos.Dtos;
using Application.Abstractions;
using Application.Repuestos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Repuestos.Controllers;

[Authorize(Policy = "Admin")]
public sealed class RepuestoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public RepuestoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RepuestoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var repuestos = await _uow.Repuestos.GetAllAsync(cancellationToken);
        return Ok(repuestos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RepuestoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        return repuesto is null ? NotFound() : Ok(Map(repuesto));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateRepuesto(
                request.CategoriaId,
                request.UnidadId,
                request.Codigo,
                request.Descripcion,
                request.StockActual,
                request.StockMinimo,
                request.PrecioUnitario),
            cancellationToken);

        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(repuesto!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRepuestoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateRepuesto(
                id,
                request.CategoriaId,
                request.UnidadId,
                request.Codigo,
                request.Descripcion,
                request.StockActual,
                request.StockMinimo,
                request.PrecioUnitario),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        if (repuesto is null)
        {
            return NotFound();
        }

        await _uow.Repuestos.RemoveAsync(repuesto, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static RepuestoDto Map(Domain.Entities.Repuesto repuesto) =>
        new(
            repuesto.Id,
            repuesto.CategoriaId,
            repuesto.UnidadId,
            repuesto.Codigo.Value,
            repuesto.Descripcion.Value,
            repuesto.StockActual,
            repuesto.StockMinimo,
            repuesto.PrecioUnitario.Value,
            repuesto.Activo);
}

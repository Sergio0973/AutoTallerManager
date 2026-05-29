using Api.Common.Controllers;
using Api.EstadosOrden.Dtos;
using Application.Abstractions;
using Application.EstadosOrden.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.EstadosOrden.Controllers;

public sealed class EstadoOrdenController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public EstadoOrdenController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EstadoOrdenDto>>> GetAll(CancellationToken cancellationToken)
    {
        var estados = await _uow.EstadosOrden.GetAllAsync(cancellationToken);
        return Ok(estados.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstadoOrdenDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosOrden.GetByIdAsync(id, cancellationToken);
        return estado is null ? NotFound() : Ok(Map(estado));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEstadoOrdenRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateEstadoOrden(request.Nombre, request.Descripcion), cancellationToken);
        var estado = await _uow.EstadosOrden.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(estado!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEstadoOrdenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateEstadoOrden(id, request.Nombre, request.Descripcion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var estado = await _uow.EstadosOrden.GetByIdAsync(id, cancellationToken);
        if (estado is null)
        {
            return NotFound();
        }

        await _uow.EstadosOrden.RemoveAsync(estado, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static EstadoOrdenDto Map(Domain.Entities.EstadoOrden estado) =>
        new(estado.Id, estado.Nombre.Value, estado.Descripcion.Value);
}

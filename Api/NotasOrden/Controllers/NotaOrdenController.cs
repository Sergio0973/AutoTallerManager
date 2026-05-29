using Api.Common.Controllers;
using Api.NotasOrden.Dtos;
using Application.Abstractions;
using Application.NotasOrden.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.NotasOrden.Controllers;

public sealed class NotaOrdenController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public NotaOrdenController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotaOrdenDto>>> GetAll(
        [FromQuery] int? ordenId,
        [FromQuery] int? usuarioId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.NotaOrden> notas;

        if (ordenId.HasValue)
        {
            notas = await _uow.NotasOrden.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (usuarioId.HasValue)
        {
            notas = await _uow.NotasOrden.GetByUsuarioIdAsync(usuarioId.Value, cancellationToken);
        }
        else
        {
            notas = await _uow.NotasOrden.GetAllAsync(cancellationToken);
        }

        return Ok(notas.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NotaOrdenDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var nota = await _uow.NotasOrden.GetByIdAsync(id, cancellationToken);
        return nota is null ? NotFound() : Ok(Map(nota));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotaOrdenRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateNotaOrden(request.OrdenId, request.UsuarioId, request.Contenido), cancellationToken);
        var nota = await _uow.NotasOrden.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(nota!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNotaOrdenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateNotaOrden(id, request.Contenido), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var nota = await _uow.NotasOrden.GetByIdAsync(id, cancellationToken);
        if (nota is null)
        {
            return NotFound();
        }

        await _uow.NotasOrden.RemoveAsync(nota, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static NotaOrdenDto Map(Domain.Entities.NotaOrden nota) =>
        new(nota.Id, nota.OrdenId, nota.UsuarioId, nota.Contenido.Value, nota.FechaNota);
}

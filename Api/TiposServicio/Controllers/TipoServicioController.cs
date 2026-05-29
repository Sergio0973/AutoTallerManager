using Api.Common.Controllers;
using Api.TiposServicio.Dtos;
using Application.Abstractions;
using Application.TiposServicio.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.TiposServicio.Controllers;

public sealed class TipoServicioController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public TipoServicioController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TipoServicioDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tipos = await _uow.TiposServicio.GetAllAsync(cancellationToken);
        return Ok(tipos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoServicioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var tipo = await _uow.TiposServicio.GetByIdAsync(id, cancellationToken);
        return tipo is null ? NotFound() : Ok(Map(tipo));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTipoServicioRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateTipoServicio(request.Nombre, request.Descripcion, request.DiasEstimados), cancellationToken);
        var tipo = await _uow.TiposServicio.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(tipo!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoServicioRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateTipoServicio(id, request.Nombre, request.Descripcion, request.DiasEstimados), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var tipo = await _uow.TiposServicio.GetByIdAsync(id, cancellationToken);
        if (tipo is null)
        {
            return NotFound();
        }

        await _uow.TiposServicio.RemoveAsync(tipo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static TipoServicioDto Map(Domain.Entities.TipoServicio tipo) =>
        new(tipo.Id, tipo.Nombre.Value, tipo.Descripcion.Value, tipo.DiasEstimados.Value);
}

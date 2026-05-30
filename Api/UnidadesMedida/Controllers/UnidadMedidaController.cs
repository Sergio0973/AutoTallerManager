using Api.Common.Controllers;
using Api.UnidadesMedida.Dtos;
using Application.Abstractions;
using Application.UnidadesMedida.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.UnidadesMedida.Controllers;

[Authorize(Policy = "Admin")]
public sealed class UnidadMedidaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public UnidadMedidaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UnidadMedidaDto>>> GetAll(CancellationToken cancellationToken)
    {
        var unidades = await _uow.UnidadesMedida.GetAllAsync(cancellationToken);
        return Ok(unidades.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnidadMedidaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var unidad = await _uow.UnidadesMedida.GetByIdAsync(id, cancellationToken);
        return unidad is null ? NotFound() : Ok(Map(unidad));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnidadMedidaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateUnidadMedida(request.Nombre, request.Abreviatura), cancellationToken);
        var unidad = await _uow.UnidadesMedida.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(unidad!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUnidadMedidaRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateUnidadMedida(id, request.Nombre, request.Abreviatura), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var unidad = await _uow.UnidadesMedida.GetByIdAsync(id, cancellationToken);
        if (unidad is null)
        {
            return NotFound();
        }

        await _uow.UnidadesMedida.RemoveAsync(unidad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UnidadMedidaDto Map(Domain.Entities.UnidadMedida unidad) =>
        new(unidad.Id, unidad.Nombre.Value, unidad.Abreviatura.Value);
}

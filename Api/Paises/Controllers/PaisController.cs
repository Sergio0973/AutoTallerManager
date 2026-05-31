using Api.Common.Controllers;
using Api.Paises.Dtos;
using Application.Abstractions;
using Application.Paises.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Paises.Controllers;

[Authorize(Policy = "Admin")]
public sealed class PaisController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public PaisController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaisDto>>> GetAll(CancellationToken cancellationToken)
    {
        var paises = await _uow.Paises.GetAllAsync(cancellationToken);
        return Ok(paises.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaisDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var pais = await _uow.Paises.GetByIdAsync(id, cancellationToken);
        return pais is null ? NotFound() : Ok(Map(pais));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaisRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreatePais(request.Nombre, request.Codigo), cancellationToken);
        var pais = await _uow.Paises.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(pais!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePaisRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdatePais(id, request.Nombre, request.Codigo), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var pais = await _uow.Paises.GetByIdAsync(id, cancellationToken);
        if (pais is null)
        {
            return NotFound();
        }

        if (await _uow.Paises.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el pais porque tiene departamentos asociados." });
        }

        await _uow.Paises.RemoveAsync(pais, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static PaisDto Map(Domain.Entities.Pais pais) =>
        new(pais.Id, pais.Nombre.Value, pais.Codigo.Value);
}

using Api.CategoriasRepuesto.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.CategoriasRepuesto.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.CategoriasRepuesto.Controllers;

public sealed class CategoriaRepuestoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public CategoriaRepuestoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaRepuestoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categorias = await _uow.CategoriasRepuesto.GetAllAsync(cancellationToken);
        return Ok(categorias.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaRepuestoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var categoria = await _uow.CategoriasRepuesto.GetByIdAsync(id, cancellationToken);
        return categoria is null ? NotFound() : Ok(Map(categoria));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaRepuestoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateCategoriaRepuesto(request.Nombre, request.Descripcion), cancellationToken);
        var categoria = await _uow.CategoriasRepuesto.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(categoria!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaRepuestoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCategoriaRepuesto(id, request.Nombre, request.Descripcion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var categoria = await _uow.CategoriasRepuesto.GetByIdAsync(id, cancellationToken);
        if (categoria is null)
        {
            return NotFound();
        }

        await _uow.CategoriasRepuesto.RemoveAsync(categoria, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CategoriaRepuestoDto Map(Domain.Entities.CategoriaRepuesto categoria) =>
        new(categoria.Id, categoria.Nombre.Value, categoria.Descripcion.Value);
}

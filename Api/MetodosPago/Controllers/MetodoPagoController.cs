using Api.Common.Controllers;
using Api.MetodosPago.Dtos;
using Application.Abstractions;
using Application.MetodosPago.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.MetodosPago.Controllers;

[Authorize]
public sealed class MetodoPagoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public MetodoPagoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "Mecanico")]
    public async Task<ActionResult<IReadOnlyList<MetodoPagoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var metodos = await _uow.MetodosPago.GetAllAsync(cancellationToken);
        return Ok(metodos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Mecanico")]
    public async Task<ActionResult<MetodoPagoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var metodo = await _uow.MetodosPago.GetByIdAsync(id, cancellationToken);
        return metodo is null ? NotFound() : Ok(Map(metodo));
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMetodoPagoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateMetodoPago(request.Nombre, request.Descripcion), cancellationToken);
        var metodo = await _uow.MetodosPago.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(metodo!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMetodoPagoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateMetodoPago(id, request.Nombre, request.Descripcion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var metodo = await _uow.MetodosPago.GetByIdAsync(id, cancellationToken);
        if (metodo is null)
        {
            return NotFound();
        }

        if (await _uow.MetodosPago.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el metodo de pago porque tiene pagos asociados." });
        }

        await _uow.MetodosPago.RemoveAsync(metodo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static MetodoPagoDto Map(Domain.Entities.MetodoPago metodo) =>
        new(metodo.Id, metodo.Nombre.Value, metodo.Descripcion.Value);
}

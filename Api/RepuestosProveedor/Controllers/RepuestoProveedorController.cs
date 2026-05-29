using Api.Common.Controllers;
using Api.RepuestosProveedor.Dtos;
using Application.Abstractions;
using Application.RepuestosProveedor.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.RepuestosProveedor.Controllers;

public sealed class RepuestoProveedorController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public RepuestoProveedorController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RepuestoProveedorDto>>> GetAll(
        [FromQuery] int? repuestoId,
        [FromQuery] int? proveedorId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.RepuestoProveedor> relaciones;

        if (repuestoId.HasValue)
        {
            relaciones = await _uow.RepuestosProveedor.GetByRepuestoIdAsync(repuestoId.Value, cancellationToken);
        }
        else if (proveedorId.HasValue)
        {
            relaciones = await _uow.RepuestosProveedor.GetByProveedorIdAsync(proveedorId.Value, cancellationToken);
        }
        else
        {
            relaciones = await _uow.RepuestosProveedor.GetAllAsync(cancellationToken);
        }

        return Ok(relaciones.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RepuestoProveedorDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var relacion = await _uow.RepuestosProveedor.GetByIdAsync(id, cancellationToken);
        return relacion is null ? NotFound() : Ok(Map(relacion));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoProveedorRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateRepuestoProveedor(
                request.RepuestoId,
                request.ProveedorId,
                request.PrecioCompra,
                request.Principal),
            cancellationToken);

        var relacion = await _uow.RepuestosProveedor.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(relacion!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRepuestoProveedorRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateRepuestoProveedor(
                id,
                request.PrecioCompra,
                request.Principal),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var relacion = await _uow.RepuestosProveedor.GetByIdAsync(id, cancellationToken);
        if (relacion is null)
        {
            return NotFound();
        }

        await _uow.RepuestosProveedor.RemoveAsync(relacion, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static RepuestoProveedorDto Map(Domain.Entities.RepuestoProveedor relacion) =>
        new(
            relacion.Id,
            relacion.RepuestoId,
            relacion.ProveedorId,
            relacion.PrecioCompra.Value,
            relacion.Principal);
}

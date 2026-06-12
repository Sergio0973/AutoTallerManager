using Api.Ciudades.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Ciudades.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Ciudades.Controllers;

[Authorize]
public sealed class CiudadController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public CiudadController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "Recepcionista")]
    public async Task<ActionResult<IReadOnlyList<CiudadDto>>> GetAll([FromQuery] int? departamentoId, CancellationToken cancellationToken)
    {
        var ciudades = departamentoId.HasValue
            ? await _uow.Ciudades.GetByDepartamentoIdAsync(departamentoId.Value, cancellationToken)
            : await _uow.Ciudades.GetAllAsync(cancellationToken);

        return Ok(ciudades.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Recepcionista")]
    public async Task<ActionResult<CiudadDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var ciudad = await _uow.Ciudades.GetByIdAsync(id, cancellationToken);
        return ciudad is null ? NotFound() : Ok(Map(ciudad));
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCiudadRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateCiudad(request.DepartamentoId, request.Nombre), cancellationToken);
        var ciudad = await _uow.Ciudades.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(ciudad!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCiudadRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCiudad(id, request.DepartamentoId, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ciudad = await _uow.Ciudades.GetByIdAsync(id, cancellationToken);
        if (ciudad is null)
        {
            return NotFound();
        }

        if (await _uow.Ciudades.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar la ciudad porque tiene proveedores o direcciones de cliente asociadas." });
        }

        await _uow.Ciudades.RemoveAsync(ciudad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CiudadDto Map(Domain.Entities.Ciudad ciudad) =>
        new(ciudad.Id, ciudad.DepartamentoId, ciudad.Nombre.Value);
}

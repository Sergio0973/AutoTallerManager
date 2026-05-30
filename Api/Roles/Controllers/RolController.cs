using Api.Common.Controllers;
using Api.Roles.Dtos;
using Application.Abstractions;
using Application.Roles.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Roles.Controllers;

[Authorize(Policy = "Admin")]
public sealed class RolController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public RolController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RolDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _uow.Roles.GetAllAsync(cancellationToken);
        return Ok(roles.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RolDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var rol = await _uow.Roles.GetByIdAsync(id, cancellationToken);
        return rol is null ? NotFound() : Ok(Map(rol));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateRolRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateRol(request.Nombre, request.Descripcion), cancellationToken);
        var rol = await _uow.Roles.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(rol!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRolRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateRol(id, request.Nombre, request.Descripcion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var rol = await _uow.Roles.GetByIdAsync(id, cancellationToken);
        if (rol is null)
        {
            return NotFound();
        }

        await _uow.Roles.RemoveAsync(rol, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static RolDto Map(Domain.Entities.Rol rol) =>
        new(rol.Id, rol.Nombre.Value, rol.Descripcion.Value);
}

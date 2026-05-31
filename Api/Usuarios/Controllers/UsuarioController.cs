using Api.Common.Controllers;
using Api.Usuarios.Dtos;
using Application.Abstractions;
using Application.Usuarios.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Usuarios.Controllers;

[Authorize(Policy = "Admin")]
public sealed class UsuarioController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public UsuarioController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> GetAll(CancellationToken cancellationToken)
    {
        var usuarios = await _uow.Usuarios.GetAllAsync(cancellationToken);
        return Ok(usuarios.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        return usuario is null ? NotFound() : Ok(Map(usuario));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateUsuario(request.RolId, request.Correo, request.Nombre, request.Contrasena), cancellationToken);
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(usuario!));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateUsuario(id, request.RolId, request.Correo, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetUsuarioPasswordRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new ResetUsuarioPassword(id, request.NuevaContrasena), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return NotFound();
        }

        if (await _uow.Usuarios.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el usuario porque tiene registros operativos o auditorias asociadas." });
        }

        await _uow.Usuarios.RemoveAsync(usuario, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UsuarioDto Map(Domain.Entities.Usuario usuario) =>
        new(usuario.Id, usuario.RolId, usuario.Correo.Value, usuario.Nombre.Value, usuario.Activo, usuario.FechaCreacion);
}

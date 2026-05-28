using Api.Common.Controllers;
using Api.Usuarios.Dtos;
using Application.Abstractions;
using Application.Usuarios.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Usuarios.Controllers;

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
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> GetAll(CancellationToken cancellationToken)
    {
        var usuarios = await _uow.Usuarios.GetAllAsync(cancellationToken);
        return Ok(usuarios.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        return usuario is null ? NotFound() : Ok(Map(usuario));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateUsuario(request.RolId, request.Correo, request.Nombre), cancellationToken);
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(usuario!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateUsuario(id, request.RolId, request.Correo, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(id, cancellationToken);
        if (usuario is null)
        {
            return NotFound();
        }

        await _uow.Usuarios.RemoveAsync(usuario, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UsuarioDto Map(Domain.Entities.Usuario usuario) =>
        new(usuario.Id, usuario.RolId, usuario.Correo.Value, usuario.Nombre.Value, usuario.Activo, usuario.FechaCreacion);
}

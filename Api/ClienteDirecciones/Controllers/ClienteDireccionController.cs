using Api.ClienteDirecciones.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.ClienteDirecciones.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.ClienteDirecciones.Controllers;

public sealed class ClienteDireccionController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ClienteDireccionController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteDireccionDto>>> GetAll([FromQuery] int? clienteId, CancellationToken cancellationToken)
    {
        var direcciones = clienteId.HasValue
            ? await _uow.ClienteDirecciones.GetByClienteIdAsync(clienteId.Value, cancellationToken)
            : await _uow.ClienteDirecciones.GetAllAsync(cancellationToken);

        return Ok(direcciones.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDireccionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var direccion = await _uow.ClienteDirecciones.GetByIdAsync(id, cancellationToken);
        return direccion is null ? NotFound() : Ok(Map(direccion));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteDireccionRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateClienteDireccion(request.ClienteId, request.CiudadId, request.Direccion, request.Principal), cancellationToken);
        var direccion = await _uow.ClienteDirecciones.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(direccion!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteDireccionRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateClienteDireccion(id, request.ClienteId, request.CiudadId, request.Direccion, request.Principal), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var direccion = await _uow.ClienteDirecciones.GetByIdAsync(id, cancellationToken);
        if (direccion is null)
        {
            return NotFound();
        }

        await _uow.ClienteDirecciones.RemoveAsync(direccion, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClienteDireccionDto Map(Domain.Entities.ClienteDireccion direccion) =>
        new(direccion.Id, direccion.ClienteId, direccion.CiudadId, direccion.Direccion.Value, direccion.Principal);
}

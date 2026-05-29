using Api.ClienteTelefonos.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.ClienteTelefonos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.ClienteTelefonos.Controllers;

public sealed class ClienteTelefonoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ClienteTelefonoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteTelefonoDto>>> GetAll([FromQuery] int? clienteId, CancellationToken cancellationToken)
    {
        var telefonos = clienteId.HasValue
            ? await _uow.ClienteTelefonos.GetByClienteIdAsync(clienteId.Value, cancellationToken)
            : await _uow.ClienteTelefonos.GetAllAsync(cancellationToken);

        return Ok(telefonos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteTelefonoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var telefono = await _uow.ClienteTelefonos.GetByIdAsync(id, cancellationToken);
        return telefono is null ? NotFound() : Ok(Map(telefono));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteTelefonoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateClienteTelefono(request.ClienteId, request.Telefono, request.Tipo), cancellationToken);
        var telefono = await _uow.ClienteTelefonos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(telefono!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteTelefonoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateClienteTelefono(id, request.ClienteId, request.Telefono, request.Tipo), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var telefono = await _uow.ClienteTelefonos.GetByIdAsync(id, cancellationToken);
        if (telefono is null)
        {
            return NotFound();
        }

        await _uow.ClienteTelefonos.RemoveAsync(telefono, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClienteTelefonoDto Map(Domain.Entities.ClienteTelefono telefono) =>
        new(telefono.Id, telefono.ClienteId, telefono.Telefono.Value, telefono.Tipo.Value);
}

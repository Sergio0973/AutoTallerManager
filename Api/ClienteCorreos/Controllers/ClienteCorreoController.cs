using Api.ClienteCorreos.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.ClienteCorreos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.ClienteCorreos.Controllers;

[Authorize(Policy = "Recepcionista")]
public sealed class ClienteCorreoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ClienteCorreoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteCorreoDto>>> GetAll([FromQuery] int? clienteId, CancellationToken cancellationToken)
    {
        var correos = clienteId.HasValue
            ? await _uow.ClienteCorreos.GetByClienteIdAsync(clienteId.Value, cancellationToken)
            : await _uow.ClienteCorreos.GetAllAsync(cancellationToken);

        return Ok(correos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteCorreoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var correo = await _uow.ClienteCorreos.GetByIdAsync(id, cancellationToken);
        return correo is null ? NotFound() : Ok(Map(correo));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteCorreoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateClienteCorreo(request.ClienteId, request.Correo, request.Principal), cancellationToken);
        var correo = await _uow.ClienteCorreos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(correo!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteCorreoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateClienteCorreo(id, request.ClienteId, request.Correo, request.Principal), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var correo = await _uow.ClienteCorreos.GetByIdAsync(id, cancellationToken);
        if (correo is null)
        {
            return NotFound();
        }

        await _uow.ClienteCorreos.RemoveAsync(correo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClienteCorreoDto Map(Domain.Entities.ClienteCorreo correo) =>
        new(correo.Id, correo.ClienteId, correo.Correo.Value, correo.Principal);
}

using Api.Clientes.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Clientes.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Clientes.Controllers;

public sealed class ClienteController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ClienteController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> GetAll(CancellationToken cancellationToken)
    {
        var clientes = await _uow.Clientes.GetAllAsync(cancellationToken);
        return Ok(clientes.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(id, cancellationToken);
        return cliente is null ? NotFound() : Ok(Map(cliente));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateCliente(request.Nombres, request.Apellidos, request.Documento), cancellationToken);
        var cliente = await _uow.Clientes.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(cliente!));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCliente(id, request.Nombres, request.Apellidos, request.Documento), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(id, cancellationToken);
        if (cliente is null)
        {
            return NotFound();
        }

        await _uow.Clientes.RemoveAsync(cliente, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClienteDto Map(Domain.Entities.Cliente cliente) =>
        new(cliente.Id, cliente.NombreCompleto.Nombres, cliente.NombreCompleto.Apellidos, cliente.DocumentoIdentidad.Value, cliente.FechaRegistro, cliente.Activo);
}

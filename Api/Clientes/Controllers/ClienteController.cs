using Api.Clientes.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Clientes.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Clientes.Controllers;

[Authorize(Policy = "Recepcionista")]
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
    public async Task<ActionResult<IReadOnlyList<ClienteDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { message = "pageNumber y pageSize deben ser mayores a cero." });
        }

        var total = await _uow.Clientes.CountAsync(search, cancellationToken);
        var clientes = await _uow.Clientes.GetPagedAsync(pageNumber, pageSize, search, cancellationToken);
        Response.Headers["X-Total-Count"] = total.ToString();

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

        if (await _uow.Clientes.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el cliente porque tiene vehiculos, telefonos, correos o direcciones asociados." });
        }

        await _uow.Clientes.RemoveAsync(cliente, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ClienteDto Map(Domain.Entities.Cliente cliente) =>
        new(cliente.Id, cliente.NombreCompleto.Nombres, cliente.NombreCompleto.Apellidos, cliente.DocumentoIdentidad.Value, cliente.FechaRegistro, cliente.Activo);
}

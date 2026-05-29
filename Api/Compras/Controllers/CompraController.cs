using Api.Common.Controllers;
using Api.Compras.Dtos;
using Application.Abstractions;
using Application.Compras.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Compras.Controllers;

public sealed class CompraController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public CompraController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CompraDto>>> GetAll([FromQuery] int? proveedorId, [FromQuery] int? usuarioId, [FromQuery] string? estado, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Compra> compras;

        if (proveedorId.HasValue)
        {
            compras = await _uow.Compras.GetByProveedorIdAsync(proveedorId.Value, cancellationToken);
        }
        else if (usuarioId.HasValue)
        {
            compras = await _uow.Compras.GetByUsuarioIdAsync(usuarioId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(estado))
        {
            compras = await _uow.Compras.GetByEstadoAsync(estado, cancellationToken);
        }
        else
        {
            compras = await _uow.Compras.GetAllAsync(cancellationToken);
        }

        return Ok(compras.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompraDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var compra = await _uow.Compras.GetByIdAsync(id, cancellationToken);
        return compra is null ? NotFound() : Ok(Map(compra));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompraRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateCompra(request.ProveedorId, request.UsuarioId, request.FechaCompra, request.Estado, request.Observaciones), cancellationToken);
        var compra = await _uow.Compras.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(compra!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompraRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCompra(id, request.ProveedorId, request.UsuarioId, request.FechaCompra, request.Estado, request.Observaciones), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var compra = await _uow.Compras.GetByIdAsync(id, cancellationToken);
        if (compra is null)
        {
            return NotFound();
        }

        await _uow.Compras.RemoveAsync(compra, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CompraDto Map(Domain.Entities.Compra compra) =>
        new(compra.Id, compra.ProveedorId, compra.UsuarioId, compra.FechaCompra, compra.Total.Value, compra.Estado.Value, compra.Observaciones?.Value);
}

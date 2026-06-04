using Api.Common.Controllers;
using Api.Repuestos.Dtos;
using Application.Abstractions;
using Application.Repuestos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Repuestos.Controllers;

[Authorize]
[EnableRateLimiting("repuestos-limit")]
public sealed class RepuestoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public RepuestoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = "Mecanico")]
    [ProducesResponseType(typeof(IReadOnlyList<RepuestoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<IReadOnlyList<RepuestoDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] int? stockMinimo = null,
        [FromQuery] bool? soloBajoStock = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { message = "pageNumber y pageSize deben ser mayores a cero." });
        }

        var total = await _uow.Repuestos.CountAsync(search, categoriaId, stockMinimo, soloBajoStock, cancellationToken);
        var repuestos = await _uow.Repuestos.GetPagedAsync(pageNumber, pageSize, search, categoriaId, stockMinimo, soloBajoStock, cancellationToken);
        Response.Headers["X-Total-Count"] = total.ToString();

        return Ok(repuestos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Mecanico")]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RepuestoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        return repuesto is null ? NotFound() : Ok(Map(repuesto));
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(typeof(RepuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateRepuesto(
                request.CategoriaId,
                request.UnidadId,
                request.Codigo,
                request.Descripcion,
                request.StockActual,
                request.StockMinimo,
                request.PrecioUnitario),
            cancellationToken);

        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(repuesto!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRepuestoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateRepuesto(
                id,
                request.CategoriaId,
                request.UnidadId,
                request.Codigo,
                request.Descripcion,
                request.StockActual,
                request.StockMinimo,
                request.PrecioUnitario),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var repuesto = await _uow.Repuestos.GetByIdAsync(id, cancellationToken);
        if (repuesto is null)
        {
            return NotFound();
        }

        if (await _uow.Repuestos.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el repuesto porque tiene compras, ordenes, proveedores o movimientos de inventario asociados." });
        }

        await _uow.Repuestos.RemoveAsync(repuesto, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static RepuestoDto Map(Domain.Entities.Repuesto repuesto) =>
        new(
            repuesto.Id,
            repuesto.CategoriaId,
            repuesto.UnidadId,
            repuesto.Codigo.Value,
            repuesto.Descripcion.Value,
            repuesto.StockActual,
            repuesto.StockMinimo,
            repuesto.PrecioUnitario.Value,
            repuesto.Activo);
}

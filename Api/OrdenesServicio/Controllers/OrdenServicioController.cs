using Api.Common.Controllers;
using Api.OrdenesServicio.Dtos;
using Application.Abstractions;
using Application.OrdenesServicio.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.OrdenesServicio.Controllers;

[Authorize(Policy = "Recepcionista")]
[EnableRateLimiting("ordenes-servicio-limit")]
public sealed class OrdenServicioController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public OrdenServicioController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrdenServicioDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? estadoId = null,
        [FromQuery] int? vehiculoId = null,
        [FromQuery] int? recepcionistaId = null,
        [FromQuery] DateOnly? fechaIngresoDesde = null,
        [FromQuery] DateOnly? fechaIngresoHasta = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { message = "pageNumber y pageSize deben ser mayores a cero." });
        }

        if (fechaIngresoDesde.HasValue && fechaIngresoHasta.HasValue && fechaIngresoDesde > fechaIngresoHasta)
        {
            return BadRequest(new { message = "fechaIngresoDesde no puede ser mayor que fechaIngresoHasta." });
        }

        var total = await _uow.OrdenesServicio.CountAsync(
            search,
            estadoId,
            vehiculoId,
            recepcionistaId,
            fechaIngresoDesde,
            fechaIngresoHasta,
            cancellationToken);

        var ordenes = await _uow.OrdenesServicio.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            estadoId,
            vehiculoId,
            recepcionistaId,
            fechaIngresoDesde,
            fechaIngresoHasta,
            cancellationToken);

        Response.Headers["X-Total-Count"] = total.ToString();

        return Ok(ordenes.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdenServicioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(id, cancellationToken);
        return orden is null ? NotFound() : Ok(Map(orden));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenServicioRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateOrdenServicio(
                request.VehiculoId,
                request.RecepcionistaId,
                request.EstadoId,
                request.CitaId,
                request.KilometrajeIngreso,
                request.FechaIngreso,
                request.FechaEstimada,
                request.Observaciones),
            cancellationToken);

        var orden = await _uow.OrdenesServicio.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(orden!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrdenServicioRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateOrdenServicio(
                id,
                request.VehiculoId,
                request.RecepcionistaId,
                request.EstadoId,
                request.CitaId,
                request.KilometrajeIngreso,
                request.FechaEstimada,
                request.FechaEntregaReal,
                request.Observaciones),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(id, cancellationToken);
        if (orden is null)
        {
            return NotFound();
        }

        if (await _uow.OrdenesServicio.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar la orden porque tiene servicios, mecanicos, tareas, detalles, notas, historial, factura, garantia o logs asociados." });
        }

        await _uow.OrdenesServicio.RemoveAsync(orden, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static OrdenServicioDto Map(Domain.Entities.OrdenServicio orden) =>
        new(
            orden.Id,
            orden.VehiculoId,
            orden.RecepcionistaId,
            orden.EstadoId,
            orden.CitaId,
            orden.KilometrajeIngreso.Value,
            orden.FechaIngreso,
            orden.FechaEstimada,
            orden.FechaEntregaReal,
            orden.Observaciones?.Value);
}

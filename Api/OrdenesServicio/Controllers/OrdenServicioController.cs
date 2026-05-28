using Api.Common.Controllers;
using Api.OrdenesServicio.Dtos;
using Application.Abstractions;
using Application.OrdenesServicio.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.OrdenesServicio.Controllers;

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
    public async Task<ActionResult<IReadOnlyList<OrdenServicioDto>>> GetAll(CancellationToken cancellationToken)
    {
        var ordenes = await _uow.OrdenesServicio.GetAllAsync(cancellationToken);
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

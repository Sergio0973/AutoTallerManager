using Api.Common.Controllers;
using Api.HistorialesKilometraje.Dtos;
using Application.Abstractions;
using Application.HistorialesKilometraje.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.HistorialesKilometraje.Controllers;

[Authorize(Policy = "Recepcionista")]
public sealed class HistorialKilometrajeController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public HistorialKilometrajeController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HistorialKilometrajeDto>>> GetAll([FromQuery] int? vehiculoId, CancellationToken cancellationToken)
    {
        var historiales = vehiculoId.HasValue
            ? await _uow.HistorialesKilometraje.GetByVehiculoIdAsync(vehiculoId.Value, cancellationToken)
            : await _uow.HistorialesKilometraje.GetAllAsync(cancellationToken);

        return Ok(historiales.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HistorialKilometrajeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var historial = await _uow.HistorialesKilometraje.GetByIdAsync(id, cancellationToken);
        return historial is null ? NotFound() : Ok(Map(historial));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHistorialKilometrajeRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateHistorialKilometraje(request.VehiculoId, request.Kilometraje, request.Fecha, request.Fuente), cancellationToken);
        var historial = await _uow.HistorialesKilometraje.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(historial!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHistorialKilometrajeRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateHistorialKilometraje(id, request.VehiculoId, request.Kilometraje, request.Fecha, request.Fuente), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var historial = await _uow.HistorialesKilometraje.GetByIdAsync(id, cancellationToken);
        if (historial is null)
        {
            return NotFound();
        }

        await _uow.HistorialesKilometraje.RemoveAsync(historial, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static HistorialKilometrajeDto Map(Domain.Entities.HistorialKilometraje historial) =>
        new(historial.Id, historial.VehiculoId, historial.Kilometraje.Value, historial.Fecha, historial.Fuente.Value);
}

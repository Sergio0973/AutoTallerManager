using Api.Common.Controllers;
using Api.TareasMecanicos.Dtos;
using Application.Abstractions;
using Application.TareasMecanicos.UseCase;
using Domain.ValueObjects.TareaMecanicos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.TareasMecanicos.Controllers;

[Authorize(Policy = "Mecanico")]
public sealed class TareaMecanicoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public TareaMecanicoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TareaMecanicoDto>>> GetAll(
        [FromQuery] int? ordenId,
        [FromQuery] int? mecanicoId,
        [FromQuery] string? estado,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.TareaMecanico> tareas;

        if (ordenId.HasValue)
        {
            tareas = await _uow.TareasMecanicos.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (mecanicoId.HasValue)
        {
            tareas = await _uow.TareasMecanicos.GetByMecanicoIdAsync(mecanicoId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(estado))
        {
            tareas = await _uow.TareasMecanicos.GetByEstadoAsync(EstadoTarea.Create(estado), cancellationToken);
        }
        else
        {
            tareas = await _uow.TareasMecanicos.GetAllAsync(cancellationToken);
        }

        return Ok(tareas.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TareaMecanicoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var tarea = await _uow.TareasMecanicos.GetByIdAsync(id, cancellationToken);
        return tarea is null ? NotFound() : Ok(Map(tarea));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTareaMecanicoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateTareaMecanico(
                request.OrdenId,
                request.MecanicoId,
                request.TipoServicioId,
                request.Descripcion,
                request.HorasTrabajadas,
                request.CostoHora,
                request.Estado,
                request.FechaInicio,
                request.FechaFin),
            cancellationToken);

        var tarea = await _uow.TareasMecanicos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(tarea!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTareaMecanicoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateTareaMecanico(
                id,
                request.Descripcion,
                request.HorasTrabajadas,
                request.CostoHora,
                request.Estado,
                request.FechaInicio,
                request.FechaFin),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var tarea = await _uow.TareasMecanicos.GetByIdAsync(id, cancellationToken);
        if (tarea is null)
        {
            return NotFound();
        }

        await _uow.TareasMecanicos.RemoveAsync(tarea, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static TareaMecanicoDto Map(Domain.Entities.TareaMecanico tarea) =>
        new(
            tarea.Id,
            tarea.OrdenId,
            tarea.MecanicoId,
            tarea.TipoServicioId,
            tarea.Descripcion.Value,
            tarea.HorasTrabajadas.Value,
            tarea.CostoHora.Value,
            tarea.HorasTrabajadas.Value * tarea.CostoHora.Value,
            tarea.Estado.Value,
            tarea.FechaInicio,
            tarea.FechaFin);
}

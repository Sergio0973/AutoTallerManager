using Api.Citas.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Citas.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Citas.Controllers;

public sealed class CitaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public CitaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CitaDto>>> GetAll(
        [FromQuery] int? vehiculoId,
        [FromQuery] int? recepcionistaId,
        [FromQuery] int? tipoServicioId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Cita> citas;

        if (vehiculoId.HasValue)
        {
            citas = await _uow.Citas.GetByVehiculoIdAsync(vehiculoId.Value, cancellationToken);
        }
        else if (recepcionistaId.HasValue)
        {
            citas = await _uow.Citas.GetByRecepcionistaIdAsync(recepcionistaId.Value, cancellationToken);
        }
        else if (tipoServicioId.HasValue)
        {
            citas = await _uow.Citas.GetByTipoServicioIdAsync(tipoServicioId.Value, cancellationToken);
        }
        else
        {
            citas = await _uow.Citas.GetAllAsync(cancellationToken);
        }

        return Ok(citas.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CitaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var cita = await _uow.Citas.GetByIdAsync(id, cancellationToken);
        return cita is null ? NotFound() : Ok(Map(cita));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCitaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateCita(
                request.VehiculoId,
                request.RecepcionistaId,
                request.TipoServicioId,
                request.FechaCita,
                request.HoraInicio,
                request.HoraFin,
                request.Estado,
                request.Observaciones),
            cancellationToken);

        var cita = await _uow.Citas.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(cita!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCitaRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateCita(
                id,
                request.VehiculoId,
                request.RecepcionistaId,
                request.TipoServicioId,
                request.FechaCita,
                request.HoraInicio,
                request.HoraFin,
                request.Estado,
                request.Observaciones),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var cita = await _uow.Citas.GetByIdAsync(id, cancellationToken);
        if (cita is null)
        {
            return NotFound();
        }

        await _uow.Citas.RemoveAsync(cita, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CitaDto Map(Domain.Entities.Cita cita) =>
        new(
            cita.Id,
            cita.VehiculoId,
            cita.RecepcionistaId,
            cita.TipoServicioId,
            cita.FechaCita,
            cita.Horario.HoraInicio,
            cita.Horario.HoraFin,
            cita.Estado.Value,
            cita.Observaciones?.Value);
}

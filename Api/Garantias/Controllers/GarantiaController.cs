using Api.Common.Controllers;
using Api.Garantias.Dtos;
using Application.Abstractions;
using Application.Garantias.UseCase;
using Domain.ValueObjects.Garantias;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Garantias.Controllers;

[Authorize(Policy = "Mecanico")]
public sealed class GarantiaController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public GarantiaController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GarantiaDto>>> GetAll([FromQuery] int? ordenId, [FromQuery] int? mecanicoId, [FromQuery] string? estado, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Garantia> garantias;

        if (ordenId.HasValue)
        {
            garantias = await _uow.Garantias.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (mecanicoId.HasValue)
        {
            garantias = await _uow.Garantias.GetByMecanicoIdAsync(mecanicoId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(estado))
        {
            garantias = await _uow.Garantias.GetByEstadoAsync(EstadoGarantia.Create(estado), cancellationToken);
        }
        else
        {
            garantias = await _uow.Garantias.GetAllAsync(cancellationToken);
        }

        return Ok(garantias.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GarantiaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var garantia = await _uow.Garantias.GetByIdAsync(id, cancellationToken);
        return garantia is null ? NotFound() : Ok(Map(garantia));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGarantiaRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateGarantia(
                request.OrdenId,
                request.TipoServicioId,
                request.MecanicoId,
                request.FechaInicio,
                request.FechaVencimiento,
                request.Condiciones,
                request.Estado),
            cancellationToken);

        var garantia = await _uow.Garantias.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(garantia!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGarantiaRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateGarantia(id, request.Estado), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var garantia = await _uow.Garantias.GetByIdAsync(id, cancellationToken);
        if (garantia is null)
        {
            return NotFound();
        }

        await _uow.Garantias.RemoveAsync(garantia, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static GarantiaDto Map(Domain.Entities.Garantia garantia) =>
        new(
            garantia.Id,
            garantia.OrdenId,
            garantia.TipoServicioId,
            garantia.MecanicoId,
            garantia.FechaInicio,
            garantia.FechaVencimiento,
            garantia.Condiciones.Value,
            garantia.Estado.Value);
}

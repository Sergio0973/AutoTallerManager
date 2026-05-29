using Api.Common.Controllers;
using Api.OrdenesMecanicos.Dtos;
using Application.Abstractions;
using Application.OrdenesMecanicos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.OrdenesMecanicos.Controllers;

public sealed class OrdenMecanicoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public OrdenMecanicoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrdenMecanicoDto>>> GetAll([FromQuery] int? ordenId, [FromQuery] int? mecanicoId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.OrdenMecanico> items;

        if (ordenId.HasValue)
        {
            items = await _uow.OrdenesMecanicos.GetByOrdenIdAsync(ordenId.Value, cancellationToken);
        }
        else if (mecanicoId.HasValue)
        {
            items = await _uow.OrdenesMecanicos.GetByMecanicoIdAsync(mecanicoId.Value, cancellationToken);
        }
        else
        {
            items = await _uow.OrdenesMecanicos.GetAllAsync(cancellationToken);
        }

        return Ok(items.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdenMecanicoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _uow.OrdenesMecanicos.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(Map(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenMecanicoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateOrdenMecanico(request.OrdenId, request.MecanicoId, request.FechaAsignacion), cancellationToken);
        var item = await _uow.OrdenesMecanicos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(item!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrdenMecanicoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateOrdenMecanico(id, request.FechaAsignacion), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteOrdenMecanico(id), cancellationToken);
        return NoContent();
    }

    private static OrdenMecanicoDto Map(Domain.Entities.OrdenMecanico item) =>
        new(item.Id, item.OrdenId, item.MecanicoId, item.FechaAsignacion);
}

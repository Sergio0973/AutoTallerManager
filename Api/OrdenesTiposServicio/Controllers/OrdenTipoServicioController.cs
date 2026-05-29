using Api.Common.Controllers;
using Api.OrdenesTiposServicio.Dtos;
using Application.Abstractions;
using Application.OrdenesTiposServicio.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.OrdenesTiposServicio.Controllers;

public sealed class OrdenTipoServicioController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public OrdenTipoServicioController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrdenTipoServicioDto>>> GetAll([FromQuery] int? ordenId, CancellationToken cancellationToken)
    {
        var items = ordenId.HasValue
            ? await _uow.OrdenesTiposServicio.GetByOrdenIdAsync(ordenId.Value, cancellationToken)
            : await _uow.OrdenesTiposServicio.GetAllAsync(cancellationToken);

        return Ok(items.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdenTipoServicioDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _uow.OrdenesTiposServicio.GetByIdAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(Map(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrdenTipoServicioRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateOrdenTipoServicio(request.OrdenId, request.TipoServicioId), cancellationToken);
        var item = await _uow.OrdenesTiposServicio.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(item!));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteOrdenTipoServicio(id), cancellationToken);
        return NoContent();
    }

    private static OrdenTipoServicioDto Map(Domain.Entities.OrdenTipoServicio item) =>
        new(item.Id, item.OrdenId, item.TipoServicioId);
}

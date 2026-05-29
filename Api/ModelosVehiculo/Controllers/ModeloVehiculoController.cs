using Api.Common.Controllers;
using Api.ModelosVehiculo.Dtos;
using Application.Abstractions;
using Application.ModelosVehiculo.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.ModelosVehiculo.Controllers;

public sealed class ModeloVehiculoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ModeloVehiculoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ModeloVehiculoDto>>> GetAll([FromQuery] int? marcaId, CancellationToken cancellationToken)
    {
        var modelos = marcaId.HasValue
            ? await _uow.ModelosVehiculo.GetByMarcaIdAsync(marcaId.Value, cancellationToken)
            : await _uow.ModelosVehiculo.GetAllAsync(cancellationToken);

        return Ok(modelos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ModeloVehiculoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var modelo = await _uow.ModelosVehiculo.GetByIdAsync(id, cancellationToken);
        return modelo is null ? NotFound() : Ok(Map(modelo));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateModeloVehiculoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateModeloVehiculo(request.MarcaId, request.Nombre, request.AnioDesde, request.AnioHasta),
            cancellationToken);

        var modelo = await _uow.ModelosVehiculo.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(modelo!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModeloVehiculoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateModeloVehiculo(id, request.MarcaId, request.Nombre, request.AnioDesde, request.AnioHasta),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var modelo = await _uow.ModelosVehiculo.GetByIdAsync(id, cancellationToken);
        if (modelo is null)
        {
            return NotFound();
        }

        await _uow.ModelosVehiculo.RemoveAsync(modelo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ModeloVehiculoDto Map(Domain.Entities.ModeloVehiculo modelo) =>
        new(modelo.Id, modelo.MarcaId, modelo.Nombre.Value, modelo.Anios.AnioDesde, modelo.Anios.AnioHasta);
}

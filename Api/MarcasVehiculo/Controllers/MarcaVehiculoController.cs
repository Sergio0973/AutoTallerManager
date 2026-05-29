using Api.Common.Controllers;
using Api.MarcasVehiculo.Dtos;
using Application.Abstractions;
using Application.MarcasVehiculo.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.MarcasVehiculo.Controllers;

public sealed class MarcaVehiculoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public MarcaVehiculoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MarcaVehiculoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var marcas = await _uow.MarcasVehiculo.GetAllAsync(cancellationToken);
        return Ok(marcas.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MarcaVehiculoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var marca = await _uow.MarcasVehiculo.GetByIdAsync(id, cancellationToken);
        return marca is null ? NotFound() : Ok(Map(marca));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMarcaVehiculoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateMarcaVehiculo(request.Nombre), cancellationToken);
        var marca = await _uow.MarcasVehiculo.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(marca!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMarcaVehiculoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateMarcaVehiculo(id, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var marca = await _uow.MarcasVehiculo.GetByIdAsync(id, cancellationToken);
        if (marca is null)
        {
            return NotFound();
        }

        await _uow.MarcasVehiculo.RemoveAsync(marca, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static MarcaVehiculoDto Map(Domain.Entities.MarcaVehiculo marca) =>
        new(marca.Id, marca.Nombre.Value);
}

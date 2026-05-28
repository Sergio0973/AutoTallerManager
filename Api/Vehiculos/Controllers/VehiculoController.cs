using Api.Common.Controllers;
using Api.Vehiculos.Dtos;
using Application.Abstractions;
using Application.Vehiculos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Vehiculos.Controllers;

public sealed class VehiculoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public VehiculoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VehiculoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var vehiculos = await _uow.Vehiculos.GetAllAsync(cancellationToken);
        return Ok(vehiculos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehiculoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var vehiculo = await _uow.Vehiculos.GetByIdAsync(id, cancellationToken);
        return vehiculo is null ? NotFound() : Ok(Map(vehiculo));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehiculoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateVehiculo(request.ClienteId, request.ModeloId, request.Vin, request.Anio, request.Placa, request.Color), cancellationToken);
        var vehiculo = await _uow.Vehiculos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(vehiculo!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVehiculoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateVehiculo(id, request.ClienteId, request.ModeloId, request.Vin, request.Anio, request.Placa, request.Color), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var vehiculo = await _uow.Vehiculos.GetByIdAsync(id, cancellationToken);
        if (vehiculo is null)
        {
            return NotFound();
        }

        await _uow.Vehiculos.RemoveAsync(vehiculo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static VehiculoDto Map(Domain.Entities.Vehiculo vehiculo) =>
        new(vehiculo.Id, vehiculo.ClienteId, vehiculo.ModeloId, vehiculo.Vin.Value, vehiculo.Anio.Value, vehiculo.Placa.Value, vehiculo.Color.Value, vehiculo.Activo);
}

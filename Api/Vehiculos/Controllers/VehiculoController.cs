using Api.Common.Controllers;
using Api.Vehiculos.Dtos;
using Application.Abstractions;
using Application.Vehiculos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Vehiculos.Controllers;

[Authorize(Policy = "Recepcionista")]
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
    public async Task<ActionResult<IReadOnlyList<VehiculoDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? clienteId = null,
        [FromQuery] string? vin = null,
        [FromQuery] string? placa = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { message = "pageNumber y pageSize deben ser mayores a cero." });
        }

        var total = await _uow.Vehiculos.CountAsync(search, clienteId, vin, placa, cancellationToken);
        var vehiculos = await _uow.Vehiculos.GetPagedAsync(pageNumber, pageSize, search, clienteId, vin, placa, cancellationToken);
        Response.Headers["X-Total-Count"] = total.ToString();

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

        if (await _uow.Vehiculos.HasDependenciesAsync(id, cancellationToken))
        {
            return Conflict(new { message = "No se puede eliminar el vehiculo porque tiene citas, ordenes de servicio o historial de kilometraje asociado." });
        }

        await _uow.Vehiculos.RemoveAsync(vehiculo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static VehiculoDto Map(Domain.Entities.Vehiculo vehiculo) =>
        new(vehiculo.Id, vehiculo.ClienteId, vehiculo.ModeloId, vehiculo.Vin.Value, vehiculo.Anio.Value, vehiculo.Placa.Value, vehiculo.Color.Value, vehiculo.Activo);
}

using Api.Common.Controllers;
using Api.Proveedores.Dtos;
using Application.Abstractions;
using Application.Proveedores.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Proveedores.Controllers;

public sealed class ProveedorController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public ProveedorController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProveedorDto>>> GetAll(
        [FromQuery] int? ciudadId,
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Proveedor> proveedores;

        if (ciudadId.HasValue)
        {
            proveedores = await _uow.Proveedores.GetByCiudadIdAsync(ciudadId.Value, cancellationToken);
        }
        else if (activo.HasValue)
        {
            proveedores = await _uow.Proveedores.GetByActivoAsync(activo.Value, cancellationToken);
        }
        else
        {
            proveedores = await _uow.Proveedores.GetAllAsync(cancellationToken);
        }

        return Ok(proveedores.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProveedorDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var proveedor = await _uow.Proveedores.GetByIdAsync(id, cancellationToken);
        return proveedor is null ? NotFound() : Ok(Map(proveedor));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProveedorRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(
            new CreateProveedor(
                request.Nombre,
                request.Nit,
                request.Telefono,
                request.Correo,
                request.CiudadId),
            cancellationToken);

        var proveedor = await _uow.Proveedores.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(proveedor!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProveedorRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(
            new UpdateProveedor(
                id,
                request.Nombre,
                request.Nit,
                request.Telefono,
                request.Correo,
                request.CiudadId,
                request.Activo),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var proveedor = await _uow.Proveedores.GetByIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound();
        }

        await _uow.Proveedores.RemoveAsync(proveedor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ProveedorDto Map(Domain.Entities.Proveedor proveedor) =>
        new(
            proveedor.Id,
            proveedor.Nombre.Value,
            proveedor.Nit.Value,
            proveedor.Telefono.Value,
            proveedor.Correo.Value,
            proveedor.CiudadId,
            proveedor.Activo);
}

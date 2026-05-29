using Api.Common.Controllers;
using Api.Departamentos.Dtos;
using Application.Abstractions;
using Application.Departamentos.UseCase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Departamentos.Controllers;

public sealed class DepartamentoController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;

    public DepartamentoController(IUnitOfWork uow, ISender sender)
    {
        _uow = uow;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartamentoDto>>> GetAll([FromQuery] int? paisId, CancellationToken cancellationToken)
    {
        var departamentos = paisId.HasValue
            ? await _uow.Departamentos.GetByPaisIdAsync(paisId.Value, cancellationToken)
            : await _uow.Departamentos.GetAllAsync(cancellationToken);

        return Ok(departamentos.Select(Map).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartamentoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var departamento = await _uow.Departamentos.GetByIdAsync(id, cancellationToken);
        return departamento is null ? NotFound() : Ok(Map(departamento));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartamentoRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(new CreateDepartamento(request.PaisId, request.Nombre), cancellationToken);
        var departamento = await _uow.Departamentos.GetByIdAsync(id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(departamento!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartamentoRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateDepartamento(id, request.PaisId, request.Nombre), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var departamento = await _uow.Departamentos.GetByIdAsync(id, cancellationToken);
        if (departamento is null)
        {
            return NotFound();
        }

        await _uow.Departamentos.RemoveAsync(departamento, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static DepartamentoDto Map(Domain.Entities.Departamento departamento) =>
        new(departamento.Id, departamento.PaisId, departamento.Nombre.Value);
}

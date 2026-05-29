using Application.Abstractions;
using Domain.ValueObjects.Ciudades;
using FluentValidation;
using MediatR;

namespace Application.Ciudades.UseCase;

public sealed record UpdateCiudad(int Id, int DepartamentoId, string Nombre) : IRequest;

public sealed class UpdateCiudadValidator : AbstractValidator<UpdateCiudad>
{
    public UpdateCiudadValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.DepartamentoId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateCiudadHandler : IRequestHandler<UpdateCiudad>
{
    private readonly IUnitOfWork _uow;

    public UpdateCiudadHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateCiudad request, CancellationToken cancellationToken)
    {
        var ciudad = await _uow.Ciudades.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Ciudad no encontrada.");

        _ = await _uow.Departamentos.GetByIdAsync(request.DepartamentoId, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento no encontrado.");

        var nombre = NombreCiudad.Create(request.Nombre);
        var existing = await _uow.Ciudades.GetByDepartamentoAndNombreAsync(request.DepartamentoId, nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe una ciudad con ese nombre para el departamento indicado.");
        }

        ciudad.Update(request.DepartamentoId, nombre);
        await _uow.Ciudades.UpdateAsync(ciudad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

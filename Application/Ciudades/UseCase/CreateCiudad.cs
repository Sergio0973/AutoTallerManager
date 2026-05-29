using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Ciudades;
using FluentValidation;
using MediatR;

namespace Application.Ciudades.UseCase;

public sealed record CreateCiudad(int DepartamentoId, string Nombre) : IRequest<int>;

public sealed class CreateCiudadValidator : AbstractValidator<CreateCiudad>
{
    public CreateCiudadValidator()
    {
        RuleFor(x => x.DepartamentoId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateCiudadHandler : IRequestHandler<CreateCiudad, int>
{
    private readonly IUnitOfWork _uow;

    public CreateCiudadHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateCiudad request, CancellationToken cancellationToken)
    {
        _ = await _uow.Departamentos.GetByIdAsync(request.DepartamentoId, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento no encontrado.");

        var nombre = NombreCiudad.Create(request.Nombre);
        if (await _uow.Ciudades.GetByDepartamentoAndNombreAsync(request.DepartamentoId, nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe una ciudad con ese nombre para el departamento indicado.");
        }

        var ciudad = new Ciudad(request.DepartamentoId, nombre);
        await _uow.Ciudades.AddAsync(ciudad, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return ciudad.Id;
    }
}

using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Departamentos;
using FluentValidation;
using MediatR;

namespace Application.Departamentos.UseCase;

public sealed record CreateDepartamento(int PaisId, string Nombre) : IRequest<int>;

public sealed class CreateDepartamentoValidator : AbstractValidator<CreateDepartamento>
{
    public CreateDepartamentoValidator()
    {
        RuleFor(x => x.PaisId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateDepartamentoHandler : IRequestHandler<CreateDepartamento, int>
{
    private readonly IUnitOfWork _uow;

    public CreateDepartamentoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateDepartamento request, CancellationToken cancellationToken)
    {
        _ = await _uow.Paises.GetByIdAsync(request.PaisId, cancellationToken)
            ?? throw new KeyNotFoundException("Pais no encontrado.");

        var nombre = NombreDepartamento.Create(request.Nombre);
        if (await _uow.Departamentos.GetByPaisAndNombreAsync(request.PaisId, nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un departamento con ese nombre para el pais indicado.");
        }

        var departamento = new Departamento(request.PaisId, nombre);
        await _uow.Departamentos.AddAsync(departamento, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return departamento.Id;
    }
}

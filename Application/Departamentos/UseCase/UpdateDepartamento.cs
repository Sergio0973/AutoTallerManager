using Application.Abstractions;
using Domain.ValueObjects.Departamentos;
using FluentValidation;
using MediatR;

namespace Application.Departamentos.UseCase;

public sealed record UpdateDepartamento(int Id, int PaisId, string Nombre) : IRequest;

public sealed class UpdateDepartamentoValidator : AbstractValidator<UpdateDepartamento>
{
    public UpdateDepartamentoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.PaisId).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateDepartamentoHandler : IRequestHandler<UpdateDepartamento>
{
    private readonly IUnitOfWork _uow;

    public UpdateDepartamentoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateDepartamento request, CancellationToken cancellationToken)
    {
        var departamento = await _uow.Departamentos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Departamento no encontrado.");

        _ = await _uow.Paises.GetByIdAsync(request.PaisId, cancellationToken)
            ?? throw new KeyNotFoundException("Pais no encontrado.");

        var nombre = NombreDepartamento.Create(request.Nombre);
        var existing = await _uow.Departamentos.GetByPaisAndNombreAsync(request.PaisId, nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un departamento con ese nombre para el pais indicado.");
        }

        departamento.Update(request.PaisId, nombre);
        await _uow.Departamentos.UpdateAsync(departamento, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

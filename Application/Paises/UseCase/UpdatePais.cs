using Application.Abstractions;
using Domain.ValueObjects.Paises;
using FluentValidation;
using MediatR;

namespace Application.Paises.UseCase;

public sealed record UpdatePais(int Id, string Nombre, string Codigo) : IRequest;

public sealed class UpdatePaisValidator : AbstractValidator<UpdatePais>
{
    public UpdatePaisValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
    }
}

public sealed class UpdatePaisHandler : IRequestHandler<UpdatePais>
{
    private readonly IUnitOfWork _uow;

    public UpdatePaisHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdatePais request, CancellationToken cancellationToken)
    {
        var pais = await _uow.Paises.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Pais no encontrado.");

        var codigo = CodigoPais.Create(request.Codigo);
        var existing = await _uow.Paises.GetByCodigoAsync(codigo, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un pais con ese codigo.");
        }

        pais.Actualizar(NombrePais.Create(request.Nombre), codigo);
        await _uow.Paises.UpdateAsync(pais, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

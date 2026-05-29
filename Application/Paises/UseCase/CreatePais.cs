using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Paises;
using FluentValidation;
using MediatR;

namespace Application.Paises.UseCase;

public sealed record CreatePais(string Nombre, string Codigo) : IRequest<int>;

public sealed class CreatePaisValidator : AbstractValidator<CreatePais>
{
    public CreatePaisValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
    }
}

public sealed class CreatePaisHandler : IRequestHandler<CreatePais, int>
{
    private readonly IUnitOfWork _uow;

    public CreatePaisHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreatePais request, CancellationToken cancellationToken)
    {
        var codigo = CodigoPais.Create(request.Codigo);
        if (await _uow.Paises.GetByCodigoAsync(codigo, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un pais con ese codigo.");
        }

        var pais = new Pais(NombrePais.Create(request.Nombre), codigo);
        await _uow.Paises.AddAsync(pais, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return pais.Id;
    }
}

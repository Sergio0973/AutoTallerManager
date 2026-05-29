using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.CategoriaRepuestos;
using FluentValidation;
using MediatR;

namespace Application.CategoriasRepuesto.UseCase;

public sealed record CreateCategoriaRepuesto(string Nombre, string Descripcion) : IRequest<int>;

public sealed class CreateCategoriaRepuestoValidator : AbstractValidator<CreateCategoriaRepuesto>
{
    public CreateCategoriaRepuestoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateCategoriaRepuestoHandler : IRequestHandler<CreateCategoriaRepuesto, int>
{
    private readonly IUnitOfWork _uow;

    public CreateCategoriaRepuestoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateCategoriaRepuesto request, CancellationToken cancellationToken)
    {
        var nombre = NombreCategoria.Create(request.Nombre);
        if (await _uow.CategoriasRepuesto.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe una categoria de repuesto con ese nombre.");
        }

        var categoria = new CategoriaRepuesto(nombre, DescripcionCategoria.Create(request.Descripcion));
        await _uow.CategoriasRepuesto.AddAsync(categoria, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return categoria.Id;
    }
}

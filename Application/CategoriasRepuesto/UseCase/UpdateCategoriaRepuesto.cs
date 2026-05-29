using Application.Abstractions;
using Domain.ValueObjects.CategoriaRepuestos;
using FluentValidation;
using MediatR;

namespace Application.CategoriasRepuesto.UseCase;

public sealed record UpdateCategoriaRepuesto(int Id, string Nombre, string Descripcion) : IRequest;

public sealed class UpdateCategoriaRepuestoValidator : AbstractValidator<UpdateCategoriaRepuesto>
{
    public UpdateCategoriaRepuestoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class UpdateCategoriaRepuestoHandler : IRequestHandler<UpdateCategoriaRepuesto>
{
    private readonly IUnitOfWork _uow;

    public UpdateCategoriaRepuestoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateCategoriaRepuesto request, CancellationToken cancellationToken)
    {
        var categoria = await _uow.CategoriasRepuesto.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de repuesto no encontrada.");

        var nombre = NombreCategoria.Create(request.Nombre);
        var existing = await _uow.CategoriasRepuesto.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe una categoria de repuesto con ese nombre.");
        }

        categoria.Update(nombre, DescripcionCategoria.Create(request.Descripcion));
        await _uow.CategoriasRepuesto.UpdateAsync(categoria, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

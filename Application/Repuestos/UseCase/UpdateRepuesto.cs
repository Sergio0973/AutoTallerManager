using Application.Abstractions;
using Domain.ValueObjects.Repuestos;
using FluentValidation;
using MediatR;

namespace Application.Repuestos.UseCase;

public sealed record UpdateRepuesto(
    int Id,
    int CategoriaId,
    int UnidadId,
    string Codigo,
    string Descripcion,
    int StockActual,
    int StockMinimo,
    decimal PrecioUnitario) : IRequest;

public sealed class UpdateRepuestoValidator : AbstractValidator<UpdateRepuesto>
{
    public UpdateRepuestoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.UnidadId).GreaterThan(0);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.StockActual).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateRepuestoHandler : IRequestHandler<UpdateRepuesto>
{
    private readonly IUnitOfWork _uow;

    public UpdateRepuestoHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateRepuesto request, CancellationToken cancellationToken)
    {
        var repuesto = await _uow.Repuestos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Repuesto no encontrado.");

        _ = await _uow.CategoriasRepuesto.GetByIdAsync(request.CategoriaId, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de repuesto no encontrada.");

        _ = await _uow.UnidadesMedida.GetByIdAsync(request.UnidadId, cancellationToken)
            ?? throw new KeyNotFoundException("Unidad de medida no encontrada.");

        var codigo = CodigoRepuesto.Create(request.Codigo);
        var existente = await _uow.Repuestos.GetByCodigoAsync(codigo, cancellationToken);
        if (existente is not null && existente.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un repuesto con ese codigo.");
        }

        repuesto.Update(
            request.CategoriaId,
            request.UnidadId,
            codigo,
            DescripcionRepuesto.Create(request.Descripcion),
            request.StockActual,
            request.StockMinimo,
            PrecioUnitario.Create(request.PrecioUnitario));

        await _uow.Repuestos.UpdateAsync(repuesto, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

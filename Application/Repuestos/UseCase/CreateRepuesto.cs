using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Repuestos;
using FluentValidation;
using MediatR;

namespace Application.Repuestos.UseCase;

public sealed record CreateRepuesto(
    int CategoriaId,
    int UnidadId,
    string Codigo,
    string Descripcion,
    int StockActual,
    int StockMinimo,
    decimal PrecioUnitario) : IRequest<int>;

public sealed class CreateRepuestoValidator : AbstractValidator<CreateRepuesto>
{
    public CreateRepuestoValidator()
    {
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.UnidadId).GreaterThan(0);
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.StockActual).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateRepuestoHandler : IRequestHandler<CreateRepuesto, int>
{
    private readonly IUnitOfWork _uow;

    public CreateRepuestoHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(CreateRepuesto request, CancellationToken cancellationToken)
    {
        _ = await _uow.CategoriasRepuesto.GetByIdAsync(request.CategoriaId, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria de repuesto no encontrada.");

        _ = await _uow.UnidadesMedida.GetByIdAsync(request.UnidadId, cancellationToken)
            ?? throw new KeyNotFoundException("Unidad de medida no encontrada.");

        var codigo = CodigoRepuesto.Create(request.Codigo);
        if (await _uow.Repuestos.ExistsCodigoAsync(codigo, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un repuesto con ese codigo.");
        }

        var repuesto = new Repuesto(
            request.CategoriaId,
            request.UnidadId,
            codigo,
            DescripcionRepuesto.Create(request.Descripcion),
            request.StockActual,
            request.StockMinimo,
            PrecioUnitario.Create(request.PrecioUnitario));

        await _uow.Repuestos.AddAsync(repuesto, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return repuesto.Id;
    }
}

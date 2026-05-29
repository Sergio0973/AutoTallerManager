using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.RepuestoProveedores;
using FluentValidation;
using MediatR;

namespace Application.RepuestosProveedor.UseCase;

public sealed record CreateRepuestoProveedor(
    int RepuestoId,
    int ProveedorId,
    decimal PrecioCompra,
    bool Principal) : IRequest<int>;

public sealed class CreateRepuestoProveedorValidator : AbstractValidator<CreateRepuestoProveedor>
{
    public CreateRepuestoProveedorValidator()
    {
        RuleFor(x => x.RepuestoId).GreaterThan(0);
        RuleFor(x => x.ProveedorId).GreaterThan(0);
        RuleFor(x => x.PrecioCompra).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateRepuestoProveedorHandler : IRequestHandler<CreateRepuestoProveedor, int>
{
    private readonly IUnitOfWork _uow;

    public CreateRepuestoProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateRepuestoProveedor request, CancellationToken cancellationToken)
    {
        _ = await _uow.Repuestos.GetByIdAsync(request.RepuestoId, cancellationToken)
            ?? throw new KeyNotFoundException("Repuesto no encontrado.");

        _ = await _uow.Proveedores.GetByIdAsync(request.ProveedorId, cancellationToken)
            ?? throw new KeyNotFoundException("Proveedor no encontrado.");

        if (await _uow.RepuestosProveedor.GetByRepuestoAndProveedorAsync(request.RepuestoId, request.ProveedorId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe esa relacion entre repuesto y proveedor.");
        }

        var repuestoProveedor = new RepuestoProveedor(
            request.RepuestoId,
            request.ProveedorId,
            PrecioCompra.Create(request.PrecioCompra),
            request.Principal);

        await _uow.RepuestosProveedor.AddAsync(repuestoProveedor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return repuestoProveedor.Id;
    }
}

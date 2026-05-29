using Application.Abstractions;
using Domain.ValueObjects.RepuestoProveedores;
using FluentValidation;
using MediatR;

namespace Application.RepuestosProveedor.UseCase;

public sealed record UpdateRepuestoProveedor(
    int Id,
    decimal PrecioCompra,
    bool Principal) : IRequest;

public sealed class UpdateRepuestoProveedorValidator : AbstractValidator<UpdateRepuestoProveedor>
{
    public UpdateRepuestoProveedorValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.PrecioCompra).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateRepuestoProveedorHandler : IRequestHandler<UpdateRepuestoProveedor>
{
    private readonly IUnitOfWork _uow;

    public UpdateRepuestoProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateRepuestoProveedor request, CancellationToken cancellationToken)
    {
        var repuestoProveedor = await _uow.RepuestosProveedor.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Relacion repuesto-proveedor no encontrada.");

        repuestoProveedor.Update(PrecioCompra.Create(request.PrecioCompra), request.Principal);
        await _uow.RepuestosProveedor.UpdateAsync(repuestoProveedor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

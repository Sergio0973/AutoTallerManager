using Application.Abstractions;
using Domain.ValueObjects.Compras;
using FluentValidation;
using MediatR;

namespace Application.Compras.UseCase;

public sealed record UpdateCompra(
    int Id,
    int ProveedorId,
    int UsuarioId,
    DateOnly FechaCompra,
    string Estado,
    string? Observaciones) : IRequest;

public sealed class UpdateCompraValidator : AbstractValidator<UpdateCompra>
{
    public UpdateCompraValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ProveedorId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Observaciones).MaximumLength(1000);
    }
}

public sealed class UpdateCompraHandler : IRequestHandler<UpdateCompra>
{
    private readonly IUnitOfWork _uow;

    public UpdateCompraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateCompra request, CancellationToken cancellationToken)
    {
        var compra = await _uow.Compras.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Compra no encontrada.");

        _ = await _uow.Proveedores.GetByIdAsync(request.ProveedorId, cancellationToken)
            ?? throw new KeyNotFoundException("Proveedor no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        compra.Update(
            request.ProveedorId,
            request.UsuarioId,
            request.FechaCompra,
            compra.Total,
            EstadoCompra.Create(request.Estado),
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesCompra.Create(request.Observaciones));

        await _uow.Compras.UpdateAsync(compra, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

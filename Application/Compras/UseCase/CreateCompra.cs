using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Compras;
using FluentValidation;
using MediatR;

namespace Application.Compras.UseCase;

public sealed record CreateCompra(
    int ProveedorId,
    int UsuarioId,
    DateOnly FechaCompra,
    string Estado,
    string? Observaciones) : IRequest<int>;

public sealed class CreateCompraValidator : AbstractValidator<CreateCompra>
{
    public CreateCompraValidator()
    {
        RuleFor(x => x.ProveedorId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Observaciones).MaximumLength(1000);
    }
}

public sealed class CreateCompraHandler : IRequestHandler<CreateCompra, int>
{
    private readonly IUnitOfWork _uow;

    public CreateCompraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateCompra request, CancellationToken cancellationToken)
    {
        _ = await _uow.Proveedores.GetByIdAsync(request.ProveedorId, cancellationToken)
            ?? throw new KeyNotFoundException("Proveedor no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var compra = new Compra(
            request.ProveedorId,
            request.UsuarioId,
            request.FechaCompra,
            TotalCompra.Create(0),
            EstadoCompra.Create(request.Estado),
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesCompra.Create(request.Observaciones));

        await _uow.Compras.AddAsync(compra, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return compra.Id;
    }
}

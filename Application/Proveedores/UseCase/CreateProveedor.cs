using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Proveedores;
using FluentValidation;
using MediatR;

namespace Application.Proveedores.UseCase;

public sealed record CreateProveedor(
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    int CiudadId) : IRequest<int>;

public sealed class CreateProveedorValidator : AbstractValidator<CreateProveedor>
{
    public CreateProveedorValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Nit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.CiudadId).GreaterThan(0);
    }
}

public sealed class CreateProveedorHandler : IRequestHandler<CreateProveedor, int>
{
    private readonly IUnitOfWork _uow;

    public CreateProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateProveedor request, CancellationToken cancellationToken)
    {
        _ = await _uow.Ciudades.GetByIdAsync(request.CiudadId, cancellationToken)
            ?? throw new KeyNotFoundException("Ciudad no encontrada.");

        var nit = Nit.Create(request.Nit);
        if (await _uow.Proveedores.GetByNitAsync(nit, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese NIT.");
        }

        var correo = CorreoProveedor.Create(request.Correo);
        if (await _uow.Proveedores.GetByCorreoAsync(correo, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese correo.");
        }

        var proveedor = new Proveedor(
            NombreProveedor.Create(request.Nombre),
            nit,
            TelefonoProveedor.Create(request.Telefono),
            correo,
            request.CiudadId);

        await _uow.Proveedores.AddAsync(proveedor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return proveedor.Id;
    }
}

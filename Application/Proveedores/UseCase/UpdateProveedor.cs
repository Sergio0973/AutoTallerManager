using Application.Abstractions;
using Domain.ValueObjects.Proveedores;
using FluentValidation;
using MediatR;

namespace Application.Proveedores.UseCase;

public sealed record UpdateProveedor(
    int Id,
    string Nombre,
    string Nit,
    string Telefono,
    string Correo,
    int CiudadId,
    bool Activo) : IRequest;

public sealed class UpdateProveedorValidator : AbstractValidator<UpdateProveedor>
{
    public UpdateProveedorValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Nit).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.CiudadId).GreaterThan(0);
    }
}

public sealed class UpdateProveedorHandler : IRequestHandler<UpdateProveedor>
{
    private readonly IUnitOfWork _uow;

    public UpdateProveedorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateProveedor request, CancellationToken cancellationToken)
    {
        var proveedor = await _uow.Proveedores.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Proveedor no encontrado.");

        _ = await _uow.Ciudades.GetByIdAsync(request.CiudadId, cancellationToken)
            ?? throw new KeyNotFoundException("Ciudad no encontrada.");

        var nit = Nit.Create(request.Nit);
        var existingNit = await _uow.Proveedores.GetByNitAsync(nit, cancellationToken);
        if (existingNit is not null && existingNit.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese NIT.");
        }

        var correo = CorreoProveedor.Create(request.Correo);
        var existingCorreo = await _uow.Proveedores.GetByCorreoAsync(correo, cancellationToken);
        if (existingCorreo is not null && existingCorreo.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un proveedor con ese correo.");
        }

        proveedor.Update(
            NombreProveedor.Create(request.Nombre),
            nit,
            TelefonoProveedor.Create(request.Telefono),
            correo,
            request.CiudadId);

        proveedor.CambiarEstado(request.Activo);

        await _uow.Proveedores.UpdateAsync(proveedor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

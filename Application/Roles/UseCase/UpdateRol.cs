using Application.Abstractions;
using Domain.ValueObjects.Roles;
using FluentValidation;
using MediatR;

namespace Application.Roles.UseCase;

public sealed record UpdateRol(int Id, string Nombre, string Descripcion) : IRequest;

public sealed class UpdateRolValidator : AbstractValidator<UpdateRol>
{
    public UpdateRolValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class UpdateRolHandler : IRequestHandler<UpdateRol>
{
    private readonly IUnitOfWork _uow;

    public UpdateRolHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateRol request, CancellationToken cancellationToken)
    {
        var rol = await _uow.Roles.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Rol no encontrado.");

        var nombre = NombreRol.Create(request.Nombre);
        var existing = await _uow.Roles.GetByNombreAsync(nombre, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre.");
        }

        rol.Update(nombre, DescripcionRol.Create(request.Descripcion));
        await _uow.Roles.UpdateAsync(rol, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

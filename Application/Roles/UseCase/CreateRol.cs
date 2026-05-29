using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Roles;
using FluentValidation;
using MediatR;

namespace Application.Roles.UseCase;

public sealed record CreateRol(string Nombre, string Descripcion) : IRequest<int>;

public sealed class CreateRolValidator : AbstractValidator<CreateRol>
{
    public CreateRolValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateRolHandler : IRequestHandler<CreateRol, int>
{
    private readonly IUnitOfWork _uow;

    public CreateRolHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateRol request, CancellationToken cancellationToken)
    {
        var nombre = NombreRol.Create(request.Nombre);
        if (await _uow.Roles.GetByNombreAsync(nombre, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un rol con ese nombre.");
        }

        var rol = new Rol(nombre, DescripcionRol.Create(request.Descripcion));
        await _uow.Roles.AddAsync(rol, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return rol.Id;
    }
}

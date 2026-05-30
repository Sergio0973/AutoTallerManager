using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Usuarios;
using FluentValidation;
using MediatR;

namespace Application.Usuarios.UseCase;

public sealed record CreateUsuario(int RolId, string Correo, string Nombre, string Contrasena) : IRequest<int>;

public sealed class CreateUsuarioValidator : AbstractValidator<CreateUsuario>
{
    public CreateUsuarioValidator()
    {
        RuleFor(x => x.RolId).GreaterThan(0);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Contrasena).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

public sealed class CreateUsuarioHandler : IRequestHandler<CreateUsuario, int>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUsuarioHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> Handle(CreateUsuario request, CancellationToken cancellationToken)
    {
        _ = await _uow.Roles.GetByIdAsync(request.RolId, cancellationToken)
            ?? throw new KeyNotFoundException("Rol no encontrado.");

        var correo = CorreoUsuario.Create(request.Correo);
        if (await _uow.Usuarios.ExistsCorreoAsync(correo, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.");
        }

        var usuario = new Usuario(
            request.RolId,
            correo,
            NombreUsuario.Create(request.Nombre),
            _passwordHasher.Hash(request.Contrasena));

        await _uow.Usuarios.AddAsync(usuario, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}

using Application.Abstractions;
using Domain.ValueObjects.Usuarios;
using FluentValidation;
using MediatR;

namespace Application.Usuarios.UseCase;

public sealed record UpdateUsuario(int Id, int RolId, string Correo, string Nombre) : IRequest;

public sealed class UpdateUsuarioValidator : AbstractValidator<UpdateUsuario>
{
    public UpdateUsuarioValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.RolId).GreaterThan(0);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateUsuarioHandler : IRequestHandler<UpdateUsuario>
{
    private readonly IUnitOfWork _uow;

    public UpdateUsuarioHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateUsuario request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var correo = CorreoUsuario.Create(request.Correo);
        var existente = await _uow.Usuarios.GetByCorreoAsync(correo, cancellationToken);
        if (existente is not null && existente.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.");
        }

        usuario.Update(request.RolId, correo, NombreUsuario.Create(request.Nombre));
        await _uow.Usuarios.UpdateAsync(usuario, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

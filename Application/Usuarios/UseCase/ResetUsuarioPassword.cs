using Application.Abstractions;
using FluentValidation;
using MediatR;

namespace Application.Usuarios.UseCase;

public sealed record ResetUsuarioPassword(int Id, string NuevaContrasena) : IRequest;

public sealed class ResetUsuarioPasswordValidator : AbstractValidator<ResetUsuarioPassword>
{
    public ResetUsuarioPasswordValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NuevaContrasena).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

public sealed class ResetUsuarioPasswordHandler : IRequestHandler<ResetUsuarioPassword>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public ResetUsuarioPasswordHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetUsuarioPassword request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        usuario.CambiarPassword(_passwordHasher.Hash(request.NuevaContrasena));

        await _uow.Usuarios.UpdateAsync(usuario, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteCorreos;
using FluentValidation;
using MediatR;

namespace Application.ClienteCorreos.UseCase;

public sealed record CreateClienteCorreo(int ClienteId, string Correo, bool Principal) : IRequest<int>;

public sealed class CreateClienteCorreoValidator : AbstractValidator<CreateClienteCorreo>
{
    public CreateClienteCorreoValidator()
    {
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.Correo).NotEmpty().MaximumLength(255);
    }
}

public sealed class CreateClienteCorreoHandler : IRequestHandler<CreateClienteCorreo, int>
{
    private readonly IUnitOfWork _uow;

    public CreateClienteCorreoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateClienteCorreo request, CancellationToken cancellationToken)
    {
        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var correo = CorreoElectronico.Create(request.Correo);
        if (await _uow.ClienteCorreos.GetByCorreoAsync(correo, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe un cliente con ese correo.");
        }

        var clienteCorreo = new ClienteCorreo(request.ClienteId, correo, request.Principal);
        await _uow.ClienteCorreos.AddAsync(clienteCorreo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return clienteCorreo.Id;
    }
}

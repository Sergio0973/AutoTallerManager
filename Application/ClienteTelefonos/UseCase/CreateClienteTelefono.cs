using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteTelefonos;
using FluentValidation;
using MediatR;

namespace Application.ClienteTelefonos.UseCase;

public sealed record CreateClienteTelefono(int ClienteId, string Telefono, string Tipo) : IRequest<int>;

public sealed class CreateClienteTelefonoValidator : AbstractValidator<CreateClienteTelefono>
{
    public CreateClienteTelefonoValidator()
    {
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Tipo).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateClienteTelefonoHandler : IRequestHandler<CreateClienteTelefono, int>
{
    private readonly IUnitOfWork _uow;

    public CreateClienteTelefonoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateClienteTelefono request, CancellationToken cancellationToken)
    {
        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var telefono = NumeroTelefono.Create(request.Telefono);
        if (await _uow.ClienteTelefonos.GetByClienteAndTelefonoAsync(request.ClienteId, telefono, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe ese telefono para el cliente indicado.");
        }

        var clienteTelefono = new ClienteTelefono(request.ClienteId, telefono, TipoTelefono.Create(request.Tipo));
        await _uow.ClienteTelefonos.AddAsync(clienteTelefono, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return clienteTelefono.Id;
    }
}

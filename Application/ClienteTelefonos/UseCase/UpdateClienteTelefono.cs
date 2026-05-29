using Application.Abstractions;
using Domain.ValueObjects.ClienteTelefonos;
using FluentValidation;
using MediatR;

namespace Application.ClienteTelefonos.UseCase;

public sealed record UpdateClienteTelefono(int Id, int ClienteId, string Telefono, string Tipo) : IRequest;

public sealed class UpdateClienteTelefonoValidator : AbstractValidator<UpdateClienteTelefono>
{
    public UpdateClienteTelefonoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Tipo).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateClienteTelefonoHandler : IRequestHandler<UpdateClienteTelefono>
{
    private readonly IUnitOfWork _uow;

    public UpdateClienteTelefonoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateClienteTelefono request, CancellationToken cancellationToken)
    {
        var clienteTelefono = await _uow.ClienteTelefonos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Telefono de cliente no encontrado.");

        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var telefono = NumeroTelefono.Create(request.Telefono);
        var existing = await _uow.ClienteTelefonos.GetByClienteAndTelefonoAsync(request.ClienteId, telefono, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe ese telefono para el cliente indicado.");
        }

        clienteTelefono.Update(request.ClienteId, telefono, TipoTelefono.Create(request.Tipo));
        await _uow.ClienteTelefonos.UpdateAsync(clienteTelefono, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

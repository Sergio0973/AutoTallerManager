using Application.Abstractions;
using Domain.ValueObjects.ClienteCorreos;
using FluentValidation;
using MediatR;

namespace Application.ClienteCorreos.UseCase;

public sealed record UpdateClienteCorreo(int Id, int ClienteId, string Correo, bool Principal) : IRequest;

public sealed class UpdateClienteCorreoValidator : AbstractValidator<UpdateClienteCorreo>
{
    public UpdateClienteCorreoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.Correo).NotEmpty().MaximumLength(255);
    }
}

public sealed class UpdateClienteCorreoHandler : IRequestHandler<UpdateClienteCorreo>
{
    private readonly IUnitOfWork _uow;

    public UpdateClienteCorreoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateClienteCorreo request, CancellationToken cancellationToken)
    {
        var clienteCorreo = await _uow.ClienteCorreos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Correo de cliente no encontrado.");

        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var correo = CorreoElectronico.Create(request.Correo);
        var existing = await _uow.ClienteCorreos.GetByCorreoAsync(correo, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un cliente con ese correo.");
        }

        clienteCorreo.Update(request.ClienteId, correo, request.Principal);
        await _uow.ClienteCorreos.UpdateAsync(clienteCorreo, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

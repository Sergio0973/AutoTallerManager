using Application.Abstractions;
using Domain.ValueObjects.ClienteDirecciones;
using FluentValidation;
using MediatR;

namespace Application.ClienteDirecciones.UseCase;

public sealed record UpdateClienteDireccion(int Id, int ClienteId, int CiudadId, string Direccion, bool Principal) : IRequest;

public sealed class UpdateClienteDireccionValidator : AbstractValidator<UpdateClienteDireccion>
{
    public UpdateClienteDireccionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.CiudadId).GreaterThan(0);
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(255);
    }
}

public sealed class UpdateClienteDireccionHandler : IRequestHandler<UpdateClienteDireccion>
{
    private readonly IUnitOfWork _uow;

    public UpdateClienteDireccionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateClienteDireccion request, CancellationToken cancellationToken)
    {
        var clienteDireccion = await _uow.ClienteDirecciones.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Direccion de cliente no encontrada.");

        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        _ = await _uow.Ciudades.GetByIdAsync(request.CiudadId, cancellationToken)
            ?? throw new KeyNotFoundException("Ciudad no encontrada.");

        var direccion = DireccionFisica.Create(request.Direccion);
        var existing = await _uow.ClienteDirecciones.GetByClienteCiudadAndDireccionAsync(request.ClienteId, request.CiudadId, direccion, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe esa direccion para el cliente indicado.");
        }

        clienteDireccion.Update(request.ClienteId, request.CiudadId, direccion, request.Principal);
        await _uow.ClienteDirecciones.UpdateAsync(clienteDireccion, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

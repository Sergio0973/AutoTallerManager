using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.ClienteDirecciones;
using FluentValidation;
using MediatR;

namespace Application.ClienteDirecciones.UseCase;

public sealed record CreateClienteDireccion(int ClienteId, int CiudadId, string Direccion, bool Principal) : IRequest<int>;

public sealed class CreateClienteDireccionValidator : AbstractValidator<CreateClienteDireccion>
{
    public CreateClienteDireccionValidator()
    {
        RuleFor(x => x.ClienteId).GreaterThan(0);
        RuleFor(x => x.CiudadId).GreaterThan(0);
        RuleFor(x => x.Direccion).NotEmpty().MaximumLength(255);
    }
}

public sealed class CreateClienteDireccionHandler : IRequestHandler<CreateClienteDireccion, int>
{
    private readonly IUnitOfWork _uow;

    public CreateClienteDireccionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateClienteDireccion request, CancellationToken cancellationToken)
    {
        _ = await _uow.Clientes.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        _ = await _uow.Ciudades.GetByIdAsync(request.CiudadId, cancellationToken)
            ?? throw new KeyNotFoundException("Ciudad no encontrada.");

        var direccion = DireccionFisica.Create(request.Direccion);
        if (await _uow.ClienteDirecciones.GetByClienteCiudadAndDireccionAsync(request.ClienteId, request.CiudadId, direccion, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Ya existe esa direccion para el cliente indicado.");
        }

        var clienteDireccion = new ClienteDireccion(request.ClienteId, request.CiudadId, direccion, request.Principal);
        await _uow.ClienteDirecciones.AddAsync(clienteDireccion, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return clienteDireccion.Id;
    }
}

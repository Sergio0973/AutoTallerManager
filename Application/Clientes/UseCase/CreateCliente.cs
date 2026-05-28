using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Clientes;
using FluentValidation;
using MediatR;

namespace Application.Clientes.UseCase;

public sealed record CreateCliente(string Nombres, string Apellidos, string Documento) : IRequest<int>;

public sealed class CreateClienteValidator : AbstractValidator<CreateCliente>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Documento).NotEmpty().Length(5, 20);
    }
}

public sealed class CreateClienteHandler : IRequestHandler<CreateCliente, int>
{
    private readonly IUnitOfWork _uow;

    public CreateClienteHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<int> Handle(CreateCliente request, CancellationToken cancellationToken)
    {
        var documento = Documento.Create(request.Documento);
        if (await _uow.Clientes.ExistsDocumentoAsync(documento, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un cliente con ese documento.");
        }

        var nombreCompleto = NombreCompleto.Create(request.Nombres, request.Apellidos);
        var cliente = new Cliente(nombreCompleto, documento);

        await _uow.Clientes.AddAsync(cliente, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return cliente.Id;
    }
}

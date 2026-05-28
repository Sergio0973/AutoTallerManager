using Application.Abstractions;
using Domain.ValueObjects.Clientes;
using FluentValidation;
using MediatR;

namespace Application.Clientes.UseCase;

public sealed record UpdateCliente(int Id, string Nombres, string Apellidos, string Documento) : IRequest;

public sealed class UpdateClienteValidator : AbstractValidator<UpdateCliente>
{
    public UpdateClienteValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombres).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Apellidos).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Documento).NotEmpty().Length(5, 20);
    }
}

public sealed class UpdateClienteHandler : IRequestHandler<UpdateCliente>
{
    private readonly IUnitOfWork _uow;

    public UpdateClienteHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateCliente request, CancellationToken cancellationToken)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var documento = Documento.Create(request.Documento);
        var existente = await _uow.Clientes.GetByDocumentoAsync(documento, cancellationToken);
        if (existente is not null && existente.Id != request.Id)
        {
            throw new InvalidOperationException("Ya existe un cliente con ese documento.");
        }

        cliente.Actualizar(NombreCompleto.Create(request.Nombres, request.Apellidos), documento);
        await _uow.Clientes.UpdateAsync(cliente, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

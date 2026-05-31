using Application.Abstractions;
using Application.Common.Orders;
using Domain.Entities;
using Domain.ValueObjects.NotaOrdenes;
using FluentValidation;
using MediatR;

namespace Application.NotasOrden.UseCase;

public sealed record CreateNotaOrden(int OrdenId, int UsuarioId, string Contenido) : IRequest<int>;

public sealed class CreateNotaOrdenValidator : AbstractValidator<CreateNotaOrden>
{
    public CreateNotaOrdenValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Contenido).NotEmpty().MaximumLength(1000);
    }
}

public sealed class CreateNotaOrdenHandler : IRequestHandler<CreateNotaOrden, int>
{
    private readonly IUnitOfWork _uow;

    public CreateNotaOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateNotaOrden request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await OrdenEstadoGuard.EnsureEditableAsync(_uow, orden, cancellationToken);

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var nota = new NotaOrden(
            request.OrdenId,
            request.UsuarioId,
            ContenidoNota.Create(request.Contenido));

        await _uow.NotasOrden.AddAsync(nota, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return nota.Id;
    }
}

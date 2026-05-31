using Application.Abstractions;
using Application.Common.Orders;
using Domain.Entities;
using Domain.ValueObjects.HistorialEstadoOrdenes;
using FluentValidation;
using MediatR;

namespace Application.HistorialesEstadoOrden.UseCase;

public sealed record CreateHistorialEstadoOrden(
    int OrdenId,
    int EstadoId,
    int UsuarioId,
    string? Observacion) : IRequest<int>;

public sealed class CreateHistorialEstadoOrdenValidator : AbstractValidator<CreateHistorialEstadoOrden>
{
    public CreateHistorialEstadoOrdenValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.EstadoId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Observacion).MaximumLength(500);
    }
}

public sealed class CreateHistorialEstadoOrdenHandler : IRequestHandler<CreateHistorialEstadoOrden, int>
{
    private readonly IUnitOfWork _uow;

    public CreateHistorialEstadoOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateHistorialEstadoOrden request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await OrdenEstadoGuard.EnsureEditableAsync(_uow, orden, cancellationToken);

        _ = await _uow.EstadosOrden.GetByIdAsync(request.EstadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de orden no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var observacion = string.IsNullOrWhiteSpace(request.Observacion)
            ? null
            : ObservacionHistorial.Create(request.Observacion);

        var historial = new HistorialEstadoOrden(
            request.OrdenId,
            request.EstadoId,
            request.UsuarioId,
            observacion);

        await _uow.HistorialEstadosOrden.AddAsync(historial, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return historial.Id;
    }
}

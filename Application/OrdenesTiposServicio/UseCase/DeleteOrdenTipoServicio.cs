using Application.Abstractions;
using FluentValidation;
using MediatR;

namespace Application.OrdenesTiposServicio.UseCase;

public sealed record DeleteOrdenTipoServicio(int Id) : IRequest;

public sealed class DeleteOrdenTipoServicioValidator : AbstractValidator<DeleteOrdenTipoServicio>
{
    public DeleteOrdenTipoServicioValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class DeleteOrdenTipoServicioHandler : IRequestHandler<DeleteOrdenTipoServicio>
{
    private readonly IUnitOfWork _uow;

    public DeleteOrdenTipoServicioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteOrdenTipoServicio request, CancellationToken cancellationToken)
    {
        var item = await _uow.OrdenesTiposServicio.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio de la orden no encontrado.");

        await _uow.OrdenesTiposServicio.RemoveAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

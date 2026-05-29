using Application.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.OrdenesTiposServicio.UseCase;

public sealed record CreateOrdenTipoServicio(int OrdenId, int TipoServicioId) : IRequest<int>;

public sealed class CreateOrdenTipoServicioValidator : AbstractValidator<CreateOrdenTipoServicio>
{
    public CreateOrdenTipoServicioValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.TipoServicioId).GreaterThan(0);
    }
}

public sealed class CreateOrdenTipoServicioHandler : IRequestHandler<CreateOrdenTipoServicio, int>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenTipoServicioHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateOrdenTipoServicio request, CancellationToken cancellationToken)
    {
        _ = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        _ = await _uow.TiposServicio.GetByIdAsync(request.TipoServicioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        if (await _uow.OrdenesTiposServicio.GetByOrdenAndTipoServicioAsync(request.OrdenId, request.TipoServicioId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("La orden ya tiene asociado ese tipo de servicio.");
        }

        var item = new OrdenTipoServicio(request.OrdenId, request.TipoServicioId);
        await _uow.OrdenesTiposServicio.AddAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}

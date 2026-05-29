using Application.Abstractions;
using FluentValidation;
using MediatR;

namespace Application.OrdenesMecanicos.UseCase;

public sealed record UpdateOrdenMecanico(int Id, DateOnly FechaAsignacion) : IRequest;

public sealed class UpdateOrdenMecanicoValidator : AbstractValidator<UpdateOrdenMecanico>
{
    public UpdateOrdenMecanicoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class UpdateOrdenMecanicoHandler : IRequestHandler<UpdateOrdenMecanico>
{
    private readonly IUnitOfWork _uow;

    public UpdateOrdenMecanicoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateOrdenMecanico request, CancellationToken cancellationToken)
    {
        var item = await _uow.OrdenesMecanicos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Mecanico de la orden no encontrado.");

        item.Update(request.FechaAsignacion);
        await _uow.OrdenesMecanicos.UpdateAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

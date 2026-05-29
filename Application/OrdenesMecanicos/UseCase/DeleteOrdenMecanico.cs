using Application.Abstractions;
using FluentValidation;
using MediatR;

namespace Application.OrdenesMecanicos.UseCase;

public sealed record DeleteOrdenMecanico(int Id) : IRequest;

public sealed class DeleteOrdenMecanicoValidator : AbstractValidator<DeleteOrdenMecanico>
{
    public DeleteOrdenMecanicoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public sealed class DeleteOrdenMecanicoHandler : IRequestHandler<DeleteOrdenMecanico>
{
    private readonly IUnitOfWork _uow;

    public DeleteOrdenMecanicoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteOrdenMecanico request, CancellationToken cancellationToken)
    {
        var item = await _uow.OrdenesMecanicos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Mecanico de la orden no encontrado.");

        await _uow.OrdenesMecanicos.RemoveAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

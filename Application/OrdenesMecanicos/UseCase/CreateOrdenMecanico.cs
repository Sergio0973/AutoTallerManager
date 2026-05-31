using Application.Abstractions;
using Application.Common.Orders;
using Application.Common.Security;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.OrdenesMecanicos.UseCase;

public sealed record CreateOrdenMecanico(int OrdenId, int MecanicoId, DateOnly FechaAsignacion) : IRequest<int>;

public sealed class CreateOrdenMecanicoValidator : AbstractValidator<CreateOrdenMecanico>
{
    public CreateOrdenMecanicoValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.MecanicoId).GreaterThan(0);
    }
}

public sealed class CreateOrdenMecanicoHandler : IRequestHandler<CreateOrdenMecanico, int>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenMecanicoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateOrdenMecanico request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await OrdenEstadoGuard.EnsureEditableAsync(_uow, orden, cancellationToken);

        await UserRoleGuard.EnsureMecanicoAsync(_uow, request.MecanicoId, cancellationToken);

        if (await _uow.OrdenesMecanicos.GetByOrdenAndMecanicoAsync(request.OrdenId, request.MecanicoId, cancellationToken) is not null)
        {
            throw new InvalidOperationException("La orden ya tiene asignado ese mecanico.");
        }

        if (await _uow.OrdenesMecanicos.HasActiveAssignmentForMecanicoAsync(request.MecanicoId, request.OrdenId, cancellationToken))
        {
            throw new InvalidOperationException("El mecanico ya tiene una orden de servicio activa.");
        }

        var item = new OrdenMecanico(request.OrdenId, request.MecanicoId, request.FechaAsignacion);
        await _uow.OrdenesMecanicos.AddAsync(item, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}

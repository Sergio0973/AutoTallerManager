using Application.Abstractions;
using Application.Common.Orders;
using Application.Common.Security;
using Domain.Entities;
using Domain.ValueObjects.Garantias;
using FluentValidation;
using MediatR;

namespace Application.Garantias.UseCase;

public sealed record CreateGarantia(
    int OrdenId,
    int TipoServicioId,
    int MecanicoId,
    DateOnly FechaInicio,
    DateOnly FechaVencimiento,
    string Condiciones,
    string Estado) : IRequest<int>;

public sealed class CreateGarantiaValidator : AbstractValidator<CreateGarantia>
{
    public CreateGarantiaValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.TipoServicioId).GreaterThan(0);
        RuleFor(x => x.MecanicoId).GreaterThan(0);
        RuleFor(x => x.FechaVencimiento).GreaterThanOrEqualTo(x => x.FechaInicio);
        RuleFor(x => x.Condiciones).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateGarantiaHandler : IRequestHandler<CreateGarantia, int>
{
    private readonly IUnitOfWork _uow;

    public CreateGarantiaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateGarantia request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await OrdenEstadoGuard.EnsureEditableAsync(_uow, orden, cancellationToken);

        _ = await _uow.TiposServicio.GetByIdAsync(request.TipoServicioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        await UserRoleGuard.EnsureMecanicoAsync(_uow, request.MecanicoId, cancellationToken);

        var garantia = new Garantia(
            request.OrdenId,
            request.TipoServicioId,
            request.MecanicoId,
            request.FechaInicio,
            request.FechaVencimiento,
            CondicionesGarantia.Create(request.Condiciones),
            EstadoGarantia.Create(request.Estado));

        await _uow.Garantias.AddAsync(garantia, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return garantia.Id;
    }
}

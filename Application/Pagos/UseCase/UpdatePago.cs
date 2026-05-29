using Application.Abstractions;
using Domain.ValueObjects.Pagos;
using FluentValidation;
using MediatR;

namespace Application.Pagos.UseCase;

public sealed record UpdatePago(int Id, string Estado) : IRequest;

public sealed class UpdatePagoValidator : AbstractValidator<UpdatePago>
{
    public UpdatePagoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdatePagoHandler : IRequestHandler<UpdatePago>
{
    private readonly IUnitOfWork _uow;

    public UpdatePagoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdatePago request, CancellationToken cancellationToken)
    {
        var pago = await _uow.Pagos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Pago no encontrado.");

        pago.Update(EstadoPago.Create(request.Estado));
        await _uow.Pagos.UpdateAsync(pago, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

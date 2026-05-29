using Application.Abstractions;
using Domain.ValueObjects.Garantias;
using FluentValidation;
using MediatR;

namespace Application.Garantias.UseCase;

public sealed record UpdateGarantia(int Id, string Estado) : IRequest;

public sealed class UpdateGarantiaValidator : AbstractValidator<UpdateGarantia>
{
    public UpdateGarantiaValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
    }
}

public sealed class UpdateGarantiaHandler : IRequestHandler<UpdateGarantia>
{
    private readonly IUnitOfWork _uow;

    public UpdateGarantiaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateGarantia request, CancellationToken cancellationToken)
    {
        var garantia = await _uow.Garantias.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Garantia no encontrada.");

        garantia.Update(EstadoGarantia.Create(request.Estado));
        await _uow.Garantias.UpdateAsync(garantia, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

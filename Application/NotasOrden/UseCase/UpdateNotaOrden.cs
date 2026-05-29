using Application.Abstractions;
using Domain.ValueObjects.NotaOrdenes;
using FluentValidation;
using MediatR;

namespace Application.NotasOrden.UseCase;

public sealed record UpdateNotaOrden(int Id, string Contenido) : IRequest;

public sealed class UpdateNotaOrdenValidator : AbstractValidator<UpdateNotaOrden>
{
    public UpdateNotaOrdenValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Contenido).NotEmpty().MaximumLength(1000);
    }
}

public sealed class UpdateNotaOrdenHandler : IRequestHandler<UpdateNotaOrden>
{
    private readonly IUnitOfWork _uow;

    public UpdateNotaOrdenHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateNotaOrden request, CancellationToken cancellationToken)
    {
        var nota = await _uow.NotasOrden.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Nota de orden no encontrada.");

        nota.Update(ContenidoNota.Create(request.Contenido));

        await _uow.NotasOrden.UpdateAsync(nota, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

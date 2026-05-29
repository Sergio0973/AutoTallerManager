using Application.Abstractions;
using Domain.ValueObjects.TareaMecanicos;
using FluentValidation;
using MediatR;

namespace Application.TareasMecanicos.UseCase;

public sealed record UpdateTareaMecanico(
    int Id,
    string Descripcion,
    decimal HorasTrabajadas,
    decimal CostoHora,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin) : IRequest;

public sealed class UpdateTareaMecanicoValidator : AbstractValidator<UpdateTareaMecanico>
{
    public UpdateTareaMecanicoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.HorasTrabajadas).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostoHora).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FechaFin)
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .When(x => x.FechaInicio.HasValue && x.FechaFin.HasValue);
    }
}

public sealed class UpdateTareaMecanicoHandler : IRequestHandler<UpdateTareaMecanico>
{
    private readonly IUnitOfWork _uow;

    public UpdateTareaMecanicoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateTareaMecanico request, CancellationToken cancellationToken)
    {
        var tarea = await _uow.TareasMecanicos.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Tarea de mecanico no encontrada.");

        tarea.Update(
            DescripcionTarea.Create(request.Descripcion),
            HorasTrabajadas.Create(request.HorasTrabajadas),
            CostoHora.Create(request.CostoHora),
            EstadoTarea.Create(request.Estado),
            request.FechaInicio,
            request.FechaFin);

        await _uow.TareasMecanicos.UpdateAsync(tarea, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

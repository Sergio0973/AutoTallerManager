using Application.Abstractions;
using Application.Common.Security;
using Domain.Entities;
using Domain.ValueObjects.TareaMecanicos;
using FluentValidation;
using MediatR;

namespace Application.TareasMecanicos.UseCase;

public sealed record CreateTareaMecanico(
    int OrdenId,
    int MecanicoId,
    int TipoServicioId,
    string Descripcion,
    decimal HorasTrabajadas,
    decimal CostoHora,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin) : IRequest<int>;

public sealed class CreateTareaMecanicoValidator : AbstractValidator<CreateTareaMecanico>
{
    public CreateTareaMecanicoValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.MecanicoId).GreaterThan(0);
        RuleFor(x => x.TipoServicioId).GreaterThan(0);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.HorasTrabajadas).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CostoHora).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FechaFin)
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .When(x => x.FechaInicio.HasValue && x.FechaFin.HasValue);
    }
}

public sealed class CreateTareaMecanicoHandler : IRequestHandler<CreateTareaMecanico, int>
{
    private readonly IUnitOfWork _uow;

    public CreateTareaMecanicoHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateTareaMecanico request, CancellationToken cancellationToken)
    {
        _ = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await UserRoleGuard.EnsureMecanicoAsync(_uow, request.MecanicoId, cancellationToken);

        _ = await _uow.TiposServicio.GetByIdAsync(request.TipoServicioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        var tarea = new TareaMecanico(
            request.OrdenId,
            request.MecanicoId,
            request.TipoServicioId,
            DescripcionTarea.Create(request.Descripcion),
            HorasTrabajadas.Create(request.HorasTrabajadas),
            CostoHora.Create(request.CostoHora),
            EstadoTarea.Create(request.Estado),
            request.FechaInicio,
            request.FechaFin);

        await _uow.TareasMecanicos.AddAsync(tarea, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return tarea.Id;
    }
}

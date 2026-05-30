using Application.Abstractions;
using Application.Common.Security;
using Domain.ValueObjects.Citas;
using FluentValidation;
using MediatR;

namespace Application.Citas.UseCase;

public sealed record UpdateCita(
    int Id,
    int VehiculoId,
    int RecepcionistaId,
    int TipoServicioId,
    DateOnly FechaCita,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Estado,
    string? Observaciones) : IRequest;

public sealed class UpdateCitaValidator : AbstractValidator<UpdateCita>
{
    public UpdateCitaValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.RecepcionistaId).GreaterThan(0);
        RuleFor(x => x.TipoServicioId).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.HoraFin).GreaterThan(x => x.HoraInicio);
        RuleFor(x => x.Observaciones).MaximumLength(500);
    }
}

public sealed class UpdateCitaHandler : IRequestHandler<UpdateCita>
{
    private readonly IUnitOfWork _uow;

    public UpdateCitaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(UpdateCita request, CancellationToken cancellationToken)
    {
        var cita = await _uow.Citas.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cita no encontrada.");

        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        await UserRoleGuard.EnsureRecepcionistaAsync(_uow, request.RecepcionistaId, cancellationToken);

        _ = await _uow.TiposServicio.GetByIdAsync(request.TipoServicioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        if (await _uow.Citas.ExistsOverlapAsync(request.VehiculoId, request.FechaCita, request.HoraInicio, request.HoraFin, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("El vehiculo ya tiene una cita en ese horario.");
        }

        cita.Update(
            request.VehiculoId,
            request.RecepcionistaId,
            request.TipoServicioId,
            request.FechaCita,
            HorarioCita.Create(request.HoraInicio, request.HoraFin),
            EstadoCita.Create(request.Estado),
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesCita.Create(request.Observaciones));

        await _uow.Citas.UpdateAsync(cita, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}

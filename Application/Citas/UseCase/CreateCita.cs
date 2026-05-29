using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Citas;
using FluentValidation;
using MediatR;

namespace Application.Citas.UseCase;

public sealed record CreateCita(
    int VehiculoId,
    int RecepcionistaId,
    int TipoServicioId,
    DateOnly FechaCita,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    string Estado,
    string? Observaciones) : IRequest<int>;

public sealed class CreateCitaValidator : AbstractValidator<CreateCita>
{
    public CreateCitaValidator()
    {
        RuleFor(x => x.VehiculoId).GreaterThan(0);
        RuleFor(x => x.RecepcionistaId).GreaterThan(0);
        RuleFor(x => x.TipoServicioId).GreaterThan(0);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(50);
        RuleFor(x => x.HoraFin).GreaterThan(x => x.HoraInicio);
        RuleFor(x => x.Observaciones).MaximumLength(500);
    }
}

public sealed class CreateCitaHandler : IRequestHandler<CreateCita, int>
{
    private readonly IUnitOfWork _uow;

    public CreateCitaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<int> Handle(CreateCita request, CancellationToken cancellationToken)
    {
        _ = await _uow.Vehiculos.GetByIdAsync(request.VehiculoId, cancellationToken)
            ?? throw new KeyNotFoundException("Vehiculo no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.RecepcionistaId, cancellationToken)
            ?? throw new KeyNotFoundException("Recepcionista no encontrado.");

        _ = await _uow.TiposServicio.GetByIdAsync(request.TipoServicioId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de servicio no encontrado.");

        if (await _uow.Citas.ExistsOverlapAsync(request.VehiculoId, request.FechaCita, request.HoraInicio, request.HoraFin, null, cancellationToken))
        {
            throw new InvalidOperationException("El vehiculo ya tiene una cita en ese horario.");
        }

        var cita = new Cita(
            request.VehiculoId,
            request.RecepcionistaId,
            request.TipoServicioId,
            request.FechaCita,
            HorarioCita.Create(request.HoraInicio, request.HoraFin),
            EstadoCita.Create(request.Estado),
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesCita.Create(request.Observaciones));

        await _uow.Citas.AddAsync(cita, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return cita.Id;
    }
}

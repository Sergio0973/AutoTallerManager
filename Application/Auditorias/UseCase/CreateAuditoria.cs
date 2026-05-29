using Application.Abstractions;
using Domain.Entities;
using Domain.ValueObjects.Auditorias;
using FluentValidation;
using MediatR;

namespace Application.Auditorias.UseCase;

public sealed record CreateAuditoria(
    int UsuarioId,
    string Entidad,
    int EntidadId,
    string TipoAccion,
    string? DatosAnteriores,
    string? DatosNuevos,
    string IpOrigen) : IRequest<long>;

public sealed class CreateAuditoriaValidator : AbstractValidator<CreateAuditoria>
{
    public CreateAuditoriaValidator()
    {
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Entidad).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EntidadId).GreaterThan(0);
        RuleFor(x => x.TipoAccion).NotEmpty().MaximumLength(50);
        RuleFor(x => x.IpOrigen).NotEmpty().MaximumLength(45);
    }
}

public sealed class CreateAuditoriaHandler : IRequestHandler<CreateAuditoria, long>
{
    private readonly IUnitOfWork _uow;

    public CreateAuditoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<long> Handle(CreateAuditoria request, CancellationToken cancellationToken)
    {
        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var datosAnteriores = string.IsNullOrWhiteSpace(request.DatosAnteriores)
            ? null
            : DatosJson.Create(request.DatosAnteriores);

        var datosNuevos = string.IsNullOrWhiteSpace(request.DatosNuevos)
            ? null
            : DatosJson.Create(request.DatosNuevos);

        var auditoria = new Auditoria(
            request.UsuarioId,
            EntidadAuditada.Create(request.Entidad),
            request.EntidadId,
            TipoAccion.Create(request.TipoAccion),
            datosAnteriores,
            datosNuevos,
            IpOrigen.Create(request.IpOrigen));

        await _uow.Auditorias.AddAsync(auditoria, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return auditoria.Id;
    }
}

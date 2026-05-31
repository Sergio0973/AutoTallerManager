using Application.Abstractions;
using Application.Common.Orders;
using Domain.Entities;
using Domain.ValueObjects.Facturas;
using FluentValidation;
using MediatR;

namespace Application.Facturas.UseCase;

public sealed record CreateFactura(
    int OrdenId,
    int EstadoFacturaId,
    int UsuarioId,
    decimal Descuento,
    decimal ImpuestoPct,
    DateOnly FechaEmision,
    string? Observaciones) : IRequest<int>;

public sealed class CreateFacturaValidator : AbstractValidator<CreateFactura>
{
    public CreateFacturaValidator()
    {
        RuleFor(x => x.OrdenId).GreaterThan(0);
        RuleFor(x => x.EstadoFacturaId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Descuento).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImpuestoPct).InclusiveBetween(0, 100);
    }
}

public sealed class CreateFacturaHandler : IRequestHandler<CreateFactura, int>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditoriaService _auditoriaService;

    public CreateFacturaHandler(IUnitOfWork uow, IAuditoriaService auditoriaService)
    {
        _uow = uow;
        _auditoriaService = auditoriaService;
    }

    public async Task<int> Handle(CreateFactura request, CancellationToken cancellationToken)
    {
        var orden = await _uow.OrdenesServicio.GetByIdAsync(request.OrdenId, cancellationToken)
            ?? throw new KeyNotFoundException("Orden de servicio no encontrada.");

        await OrdenEstadoGuard.EnsureCanBeFacturadaAsync(_uow, orden, cancellationToken);

        _ = await _uow.EstadosFactura.GetByIdAsync(request.EstadoFacturaId, cancellationToken)
            ?? throw new KeyNotFoundException("Estado de factura no encontrado.");

        _ = await _uow.Usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        var existente = await _uow.Facturas.GetByOrdenIdAsync(request.OrdenId, cancellationToken);
        if (existente is not null)
        {
            throw new InvalidOperationException("La orden ya tiene una factura asociada.");
        }

        var tareas = await _uow.TareasMecanicos.GetByOrdenIdAsync(request.OrdenId, cancellationToken);
        var detalles = await _uow.DetallesOrden.GetByOrdenIdAsync(request.OrdenId, cancellationToken);

        var manoDeObra = tareas.Sum(tarea => tarea.HorasTrabajadas.Value * tarea.CostoHora.Value);
        var costoRepuestos = detalles.Sum(detalle => detalle.Cantidad.Value * detalle.PrecioSnapshot.Value);
        var subtotal = manoDeObra + costoRepuestos;

        if (request.Descuento > subtotal)
        {
            throw new InvalidOperationException("El descuento no puede superar el subtotal de la factura.");
        }

        var baseImponible = subtotal - request.Descuento;
        var total = baseImponible + baseImponible * request.ImpuestoPct / 100m;

        var factura = new Factura(
            request.OrdenId,
            request.EstadoFacturaId,
            request.UsuarioId,
            ValoresFactura.Create(
                manoDeObra,
                costoRepuestos,
                request.Descuento,
                request.ImpuestoPct,
                subtotal,
                total),
            request.FechaEmision,
            string.IsNullOrWhiteSpace(request.Observaciones) ? null : ObservacionesFactura.Create(request.Observaciones));

        await _uow.Facturas.AddAsync(factura, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _auditoriaService.RegistrarAsync(
            request.UsuarioId,
            "Factura",
            factura.Id,
            "CREAR",
            null,
            new
            {
                factura.Id,
                factura.OrdenId,
                factura.EstadoFacturaId,
                factura.UsuarioId,
                factura.Valores.ManoDeObra,
                factura.Valores.CostoRepuestos,
                factura.Valores.Descuento,
                factura.Valores.ImpuestoPct,
                factura.Valores.Subtotal,
                factura.Valores.Total,
                factura.FechaEmision
            },
            cancellationToken);

        return factura.Id;
    }
}

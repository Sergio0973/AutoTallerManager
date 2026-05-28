using Domain.Entities;
using Domain.ValueObjects.OrdenServicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.ToTable("OrdenesServicio");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.KilometrajeIngreso)
            .HasConversion(
                k => k.Value,
                value => KilometrajeIngreso.Create(value))
            .HasColumnName("KilometrajeIngreso")
            .IsRequired();

        builder.Property(o => o.Observaciones)
            .HasConversion(
                obs => obs != null ? obs.Value : null,
                value => value != null ? ObservacionesOrden.Create(value) : null)
            .HasColumnName("Observaciones")
            .HasMaxLength(500);

        builder.Property(o => o.FechaIngreso).IsRequired();
        builder.Property(o => o.FechaEstimada);

        builder.HasOne(o => o.Vehiculo)
            .WithMany(v => v.OrdeneServicio)
            .HasForeignKey(o => o.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Recepcionista)
            .WithMany(u => u.OrdenesServicio)
            .HasForeignKey(o => o.RecepcionistaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Estado)
            .WithMany(e => e.OrdenesServicio)
            .HasForeignKey(o => o.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Detalles)
            .WithOne(d => d.Orden)
            .HasForeignKey(d => d.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.TiposServicio)
            .WithOne(t => t.Orden)
            .HasForeignKey(t => t.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Mecanicos)
            .WithOne(m => m.Orden)
            .HasForeignKey(m => m.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Tareas)
            .WithOne(t => t.Orden)
            .HasForeignKey(t => t.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Notas)
            .WithOne(n => n.Orden)
            .HasForeignKey(n => n.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.HistorialEstados)
            .WithOne(h => h.Orden)
            .HasForeignKey(h => h.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

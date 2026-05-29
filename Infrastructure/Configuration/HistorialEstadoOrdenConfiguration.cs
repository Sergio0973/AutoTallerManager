using Domain.Entities;
using Domain.ValueObjects.HistorialEstadoOrdenes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class HistorialEstadoOrdenConfiguration : IEntityTypeConfiguration<HistorialEstadoOrden>
{
    public void Configure(EntityTypeBuilder<HistorialEstadoOrden> builder)
    {
        builder.ToTable("HistorialEstadosOrden");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.FechaCambio)
            .HasColumnName("FechaCambio")
            .IsRequired();

        builder.Property(h => h.Observacion)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? ObservacionHistorial.Create(value) : null)
            .HasColumnName("Observacion")
            .HasMaxLength(500);

        builder.HasIndex(h => new { h.OrdenId, h.FechaCambio });
        builder.HasIndex(h => h.EstadoId);
        builder.HasIndex(h => h.UsuarioId);

        builder.HasOne(h => h.Orden)
            .WithMany(o => o.HistorialEstados)
            .HasForeignKey(h => h.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.Estado)
            .WithMany(e => e.Historiales)
            .HasForeignKey(h => h.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Usuario)
            .WithMany(u => u.HistorialEstados)
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Domain.ValueObjects.HistorialKilometrajes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class HistorialKilometrajeConfiguration : IEntityTypeConfiguration<HistorialKilometraje>
{
    public void Configure(EntityTypeBuilder<HistorialKilometraje> builder)
    {
        builder.ToTable("HistorialesKilometraje");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Kilometraje)
            .HasConversion(value => value.Value, value => Kilometraje.Create(value))
            .HasColumnName("Kilometraje")
            .IsRequired();

        builder.Property(h => h.Fecha)
            .HasColumnName("Fecha")
            .IsRequired();

        builder.Property(h => h.Fuente)
            .HasConversion(value => value.Value, value => FuenteRegistro.Create(value))
            .HasColumnName("Fuente")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(h => new { h.VehiculoId, h.Fecha });

        builder.HasOne(h => h.Vehiculo)
            .WithMany(v => v.HistorialKilometrajes)
            .HasForeignKey(h => h.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

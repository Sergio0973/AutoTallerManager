using Domain.Entities;
using Domain.ValueObjects.Citas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");
        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.Horario, horario =>
        {
            horario.Property(x => x.HoraInicio)
                .HasColumnName("HoraInicio")
                .IsRequired();

            horario.Property(x => x.HoraFin)
                .HasColumnName("HoraFin")
                .IsRequired();
        });

        builder.Property(c => c.Estado)
            .HasConversion(value => value.Value, value => EstadoCita.Create(value))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Observaciones)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? ObservacionesCita.Create(value) : null)
            .HasMaxLength(500);

        builder.HasOne(c => c.Vehiculo)
            .WithMany(v => v.Citas)
            .HasForeignKey(c => c.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Recepcionista)
            .WithMany(u => u.Citas)
            .HasForeignKey(c => c.RecepcionistaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.TipoServicio)
            .WithMany(t => t.Citas)
            .HasForeignKey(c => c.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

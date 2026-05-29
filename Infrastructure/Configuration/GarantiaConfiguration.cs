using Domain.Entities;
using Domain.ValueObjects.Garantias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class GarantiaConfiguration : IEntityTypeConfiguration<Garantia>
{
    public void Configure(EntityTypeBuilder<Garantia> builder)
    {
        builder.ToTable("Garantias");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.FechaInicio)
            .HasColumnName("FechaInicio")
            .IsRequired();

        builder.Property(g => g.FechaVencimiento)
            .HasColumnName("FechaVencimiento")
            .IsRequired();

        builder.Property(g => g.Condiciones)
            .HasConversion(value => value.Value, value => CondicionesGarantia.Create(value))
            .HasColumnName("Condiciones")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(g => g.Estado)
            .HasConversion(value => value.Value, value => EstadoGarantia.Create(value))
            .HasColumnName("Estado")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(g => g.OrdenId);
        builder.HasIndex(g => g.TipoServicioId);
        builder.HasIndex(g => g.MecanicoId);
        builder.HasIndex(g => g.FechaVencimiento);

        builder.HasOne(g => g.Orden)
            .WithMany(o => o.Garantias)
            .HasForeignKey(g => g.OrdenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.TipoServicio)
            .WithMany(t => t.Garantias)
            .HasForeignKey(g => g.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Mecanico)
            .WithMany(u => u.GarantiasMecanico)
            .HasForeignKey(g => g.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

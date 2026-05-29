using Domain.Entities;
using Domain.ValueObjects.Pagos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Monto)
            .HasConversion(value => value.Value, value => MontoPago.Create(value))
            .HasColumnName("Monto")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.FechaPago)
            .HasColumnName("FechaPago")
            .IsRequired();

        builder.Property(p => p.Referencia)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? ReferenciaPago.Create(value) : null)
            .HasColumnName("Referencia")
            .HasMaxLength(100);

        builder.Property(p => p.Estado)
            .HasConversion(value => value.Value, value => EstadoPago.Create(value))
            .HasColumnName("Estado")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => p.FacturaId);
        builder.HasIndex(p => p.MetodoPagoId);
        builder.HasIndex(p => p.FechaPago);
        builder.HasIndex(p => p.Referencia);

        builder.HasOne(p => p.Factura)
            .WithMany(f => f.Pagos)
            .HasForeignKey(p => p.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.MetodoPago)
            .WithMany(m => m.Pagos)
            .HasForeignKey(p => p.MetodoPagoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

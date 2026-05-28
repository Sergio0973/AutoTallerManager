using Domain.Entities;
using Domain.ValueObjects.Facturas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");
        builder.HasKey(f => f.Id);

        builder.OwnsOne(f => f.Valores, v =>
        {
            v.Property(p => p.ManoDeObra).HasColumnName("ManoDeObra").HasColumnType("decimal(18,2)").IsRequired();
            v.Property(p => p.CostoRepuestos).HasColumnName("CostoRepuestos").HasColumnType("decimal(18,2)").IsRequired();
            v.Property(p => p.Descuento).HasColumnName("Descuento").HasColumnType("decimal(18,2)").IsRequired();
            v.Property(p => p.ImpuestoPct).HasColumnName("ImpuestoPct").HasColumnType("decimal(5,2)").IsRequired();
            v.Property(p => p.Subtotal).HasColumnName("Subtotal").HasColumnType("decimal(18,2)").IsRequired();
            v.Property(p => p.Total).HasColumnName("Total").HasColumnType("decimal(18,2)").IsRequired();
        });

        builder.Property(f => f.Observaciones)
            .HasConversion(
                obs => obs != null ? obs.Value : null,
                value => value != null ? ObservacionesFactura.Create(value) : null)
            .HasColumnName("Observaciones")
            .HasMaxLength(500);

        builder.Property(f => f.FechaEmision).IsRequired();

        builder.HasOne(f => f.Orden)
            .WithMany(o => o.Facturas)
            .HasForeignKey(f => f.OrdenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.EstadoFactura)
            .WithMany(e => e.Facturas)
            .HasForeignKey(f => f.EstadoFacturaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Usuario)
            .WithMany(u => u.FacturasEmitidas)
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Pagos)
            .WithOne(p => p.Factura)
            .HasForeignKey(p => p.FacturaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Domain.Entities;
using Domain.ValueObjects.DetalleOrdenes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class DetalleOrdenConfiguration : IEntityTypeConfiguration<DetalleOrden>
{
    public void Configure(EntityTypeBuilder<DetalleOrden> builder)
    {
        builder.ToTable("DetallesOrden");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad)
            .HasConversion(value => value.Value, value => CantidadOrden.Create(value))
            .HasColumnName("Cantidad")
            .IsRequired();

        builder.Property(d => d.PrecioSnapshot)
            .HasConversion(value => value.Value, value => PrecioSnapshot.Create(value))
            .HasColumnName("PrecioSnapshot")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasIndex(d => new { d.OrdenId, d.RepuestoId })
            .IsUnique();

        builder.HasOne(d => d.Orden)
            .WithMany(o => o.Detalles)
            .HasForeignKey(d => d.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Repuesto)
            .WithMany(r => r.DetallesOrden)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

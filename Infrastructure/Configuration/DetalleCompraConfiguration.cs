using Domain.Entities;
using Domain.ValueObjects.DetalleCompras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class DetalleCompraConfiguration : IEntityTypeConfiguration<DetalleCompra>
{
    public void Configure(EntityTypeBuilder<DetalleCompra> builder)
    {
        builder.ToTable("DetallesCompra");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad)
            .HasConversion(value => value.Value, value => CantidadCompra.Create(value))
            .HasColumnName("Cantidad")
            .IsRequired();

        builder.Property(d => d.PrecioUnitario)
            .HasConversion(value => value.Value, value => PrecioUnitarioCompra.Create(value))
            .HasColumnName("PrecioUnitario")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasIndex(d => new { d.CompraId, d.RepuestoId })
            .IsUnique();

        builder.HasOne(d => d.Compra)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Repuesto)
            .WithMany(r => r.DetallesCompra)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

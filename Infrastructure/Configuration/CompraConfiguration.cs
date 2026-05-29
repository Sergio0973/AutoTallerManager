using Domain.Entities;
using Domain.ValueObjects.Compras;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder.ToTable("Compras");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FechaCompra)
            .HasColumnName("FechaCompra")
            .IsRequired();

        builder.Property(c => c.Total)
            .HasConversion(value => value.Value, value => TotalCompra.Create(value))
            .HasColumnName("Total")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.Estado)
            .HasConversion(value => value.Value, value => EstadoCompra.Create(value))
            .HasColumnName("Estado")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Observaciones)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? ObservacionesCompra.Create(value) : null)
            .HasColumnName("Observaciones")
            .HasMaxLength(1000);

        builder.HasIndex(c => c.ProveedorId);
        builder.HasIndex(c => c.UsuarioId);
        builder.HasIndex(c => c.FechaCompra);

        builder.HasOne(c => c.Proveedor)
            .WithMany(p => p.Compras)
            .HasForeignKey(c => c.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Compras)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Detalles)
            .WithOne(d => d.Compra)
            .HasForeignKey(d => d.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.LogsInventario)
            .WithOne(l => l.Compra)
            .HasForeignKey(l => l.CompraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

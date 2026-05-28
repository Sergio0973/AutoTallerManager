using Domain.Entities;
using Domain.ValueObjects.Repuestos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.ToTable("Repuestos");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Codigo)
            .HasConversion(
                codigo => codigo.Value,
                value => CodigoRepuesto.Create(value))
            .HasColumnName("Codigo")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Descripcion)
            .HasConversion(
                desc => desc.Value,
                value => DescripcionRepuesto.Create(value))
            .HasColumnName("Descripcion")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.PrecioUnitario)
            .HasConversion(
                precio => precio.Value,
                value => PrecioUnitario.Create(value))
            .HasColumnName("PrecioUnitario")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.StockActual).IsRequired();
        builder.Property(r => r.StockMinimo).IsRequired();
        builder.Property(r => r.Activo).IsRequired();

        builder.HasIndex(r => r.Codigo)
            .IsUnique();

        builder.HasMany(r => r.Proveedores)
            .WithOne(p => p.Repuesto)
            .HasForeignKey(p => p.RepuestoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.DetallesCompra)
            .WithOne(d => d.Repuesto)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.DetallesOrden)
            .WithOne(d => d.Repuesto)
            .HasForeignKey(d => d.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.LogsInventario)
            .WithOne(l => l.Repuesto)
            .HasForeignKey(l => l.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

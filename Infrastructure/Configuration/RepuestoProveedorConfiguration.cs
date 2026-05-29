using Domain.Entities;
using Domain.ValueObjects.RepuestoProveedores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class RepuestoProveedorConfiguration : IEntityTypeConfiguration<RepuestoProveedor>
{
    public void Configure(EntityTypeBuilder<RepuestoProveedor> builder)
    {
        builder.ToTable("RepuestosProveedor");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.PrecioCompra)
            .HasConversion(value => value.Value, value => PrecioCompra.Create(value))
            .HasColumnName("PrecioCompra")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(r => r.Principal)
            .HasColumnName("Principal")
            .IsRequired();

        builder.HasIndex(r => new { r.RepuestoId, r.ProveedorId })
            .IsUnique();

        builder.HasOne(r => r.Repuesto)
            .WithMany(repuesto => repuesto.Proveedores)
            .HasForeignKey(r => r.RepuestoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Proveedor)
            .WithMany(proveedor => proveedor.RepuestosProveedor)
            .HasForeignKey(r => r.ProveedorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Domain.Entities;
using Domain.ValueObjects.Proveedores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasConversion(value => value.Value, value => NombreProveedor.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Nit)
            .HasConversion(value => value.Value, value => Nit.Create(value))
            .HasColumnName("Nit")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Telefono)
            .HasConversion(value => value.Value, value => TelefonoProveedor.Create(value))
            .HasColumnName("Telefono")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Correo)
            .HasConversion(value => value.Value, value => CorreoProveedor.Create(value))
            .HasColumnName("Correo")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Activo)
            .HasColumnName("Activo")
            .IsRequired();

        builder.HasIndex(p => p.Nit)
            .IsUnique();

        builder.HasIndex(p => p.Correo)
            .IsUnique();

        builder.HasOne(p => p.Ciudad)
            .WithMany(c => c.Proveedores)
            .HasForeignKey(p => p.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Compras)
            .WithOne(c => c.Proveedor)
            .HasForeignKey(c => c.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.RepuestosProveedor)
            .WithOne(r => r.Proveedor)
            .HasForeignKey(r => r.ProveedorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

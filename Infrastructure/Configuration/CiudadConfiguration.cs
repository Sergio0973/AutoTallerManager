using Domain.Entities;
using Domain.ValueObjects.Ciudades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class CiudadConfiguration : IEntityTypeConfiguration<Ciudad>
{
    public void Configure(EntityTypeBuilder<Ciudad> builder)
    {
        builder.ToTable("Ciudades");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .HasConversion(value => value.Value, value => NombreCiudad.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(c => new { c.DepartamentoId, c.Nombre })
            .IsUnique();

        builder.HasOne(c => c.Departamento)
            .WithMany(d => d.Ciudades)
            .HasForeignKey(c => c.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.DireccionesCliente)
            .WithOne(d => d.Ciudad)
            .HasForeignKey(d => d.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Proveedores)
            .WithOne(p => p.Ciudad)
            .HasForeignKey(p => p.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Domain.ValueObjects.Paises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("Paises");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasConversion(value => value.Value, value => NombrePais.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Codigo)
            .HasConversion(value => value.Value, value => CodigoPais.Create(value))
            .HasColumnName("Codigo")
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(p => p.Codigo)
            .IsUnique();

        builder.HasMany(p => p.Departamentos)
            .WithOne(d => d.Pais)
            .HasForeignKey(d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

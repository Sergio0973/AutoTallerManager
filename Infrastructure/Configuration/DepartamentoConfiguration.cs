using Domain.Entities;
using Domain.ValueObjects.Departamentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.ToTable("Departamentos");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nombre)
            .HasConversion(value => value.Value, value => NombreDepartamento.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(d => new { d.PaisId, d.Nombre })
            .IsUnique();

        builder.HasOne(d => d.Pais)
            .WithMany(p => p.Departamentos)
            .HasForeignKey(d => d.PaisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Ciudades)
            .WithOne(c => c.Departamento)
            .HasForeignKey(c => c.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

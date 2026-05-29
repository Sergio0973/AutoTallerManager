using Domain.Entities;
using Domain.ValueObjects.MarcaVehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class MarcaVehiculoConfiguration : IEntityTypeConfiguration<MarcaVehiculo>
{
    public void Configure(EntityTypeBuilder<MarcaVehiculo> builder)
    {
        builder.ToTable("MarcasVehiculo");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .HasConversion(value => value.Value, value => NombreMarca.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(m => m.Nombre)
            .IsUnique();

        builder.HasMany(m => m.Modelos)
            .WithOne(modelo => modelo.Marca)
            .HasForeignKey(modelo => modelo.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

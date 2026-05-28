using Domain.Entities;
using Domain.ValueObjects.Vehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.ToTable("Vehiculos");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Vin)
            .HasConversion(vin => vin.Value, value => Vin.Create(value))
            .HasColumnName("Vin")
            .HasMaxLength(17)
            .IsRequired();
        builder.Property(v => v.Placa)
            .HasConversion(p => p.Value, v => Placa.Create(v))
            .HasColumnName("Placa")
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(v => v.Anio)
            .HasConversion(a => a.Value, v => AnioVehiculo.Create(v))
            .HasColumnName("Anio")
            .IsRequired();
        builder.Property(v => v.Color)
            .HasConversion(c => c.Value, v => Color.Create(v))
            .HasColumnName("Color")
            .HasMaxLength(30);

        builder.HasIndex(v => v.Vin)
            .IsUnique();

        builder.HasIndex(v => v.Placa)
            .IsUnique();

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vehiculos)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

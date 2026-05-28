using Domain.Entities;
using Domain.ValueObjects.ModeloVehiculos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class ModeloVehiculoConfiguration : IEntityTypeConfiguration<ModeloVehiculo>
{
    public void Configure(EntityTypeBuilder<ModeloVehiculo> builder)
    {
        builder.ToTable("ModelosVehiculo");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .HasConversion(value => value.Value, value => NombreModelo.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(m => m.Anios, anios =>
        {
            anios.Property(x => x.AnioDesde)
                .HasColumnName("AnioDesde")
                .IsRequired();

            anios.Property(x => x.AnioHasta)
                .HasColumnName("AnioHasta")
                .IsRequired();
        });

        builder.HasOne(m => m.Marca)
            .WithMany(marca => marca.Modelos)
            .HasForeignKey(m => m.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

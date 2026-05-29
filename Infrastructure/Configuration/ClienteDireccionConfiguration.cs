using Domain.Entities;
using Domain.ValueObjects.ClienteDirecciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class ClienteDireccionConfiguration : IEntityTypeConfiguration<ClienteDireccion>
{
    public void Configure(EntityTypeBuilder<ClienteDireccion> builder)
    {
        builder.ToTable("ClienteDirecciones");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Direccion)
            .HasConversion(value => value.Value, value => DireccionFisica.Create(value))
            .HasColumnName("Direccion")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.Principal)
            .HasColumnName("Principal")
            .IsRequired();

        builder.HasOne(d => d.Cliente)
            .WithMany(c => c.Direcciones)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Ciudad)
            .WithMany(c => c.DireccionesCliente)
            .HasForeignKey(d => d.CiudadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

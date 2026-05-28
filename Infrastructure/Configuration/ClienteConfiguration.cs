using Domain.Entities;
using Domain.ValueObjects.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.NombreCompleto, nc =>
        {
            nc.Property(n => n.Nombres)
                .HasColumnName("Nombres")
                .HasMaxLength(100)
                .IsRequired();

            nc.Property(n => n.Apellidos)
                .HasColumnName("Apellidos")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(c => c.DocumentoIdentidad)
            .HasConversion(
                doc => doc.Value,
                value => Documento.Create(value))
            .HasColumnName("DocumentoIdentidad")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.FechaRegistro)
            .HasColumnName("FechaRegistro")
            .IsRequired();

        builder.HasIndex(c => c.DocumentoIdentidad)
            .IsUnique();

        builder.HasMany(c => c.Vehiculos)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Direcciones)
            .WithOne(d => d.Cliente)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Telefonos)
            .WithOne(t => t.Cliente)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Correos)
            .WithOne(co => co.Cliente)
            .HasForeignKey(co => co.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

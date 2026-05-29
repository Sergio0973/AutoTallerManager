using Domain.Entities;
using Domain.ValueObjects.ClienteTelefonos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class ClienteTelefonoConfiguration : IEntityTypeConfiguration<ClienteTelefono>
{
    public void Configure(EntityTypeBuilder<ClienteTelefono> builder)
    {
        builder.ToTable("ClienteTelefonos");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Telefono)
            .HasConversion(value => value.Value, value => NumeroTelefono.Create(value))
            .HasColumnName("Telefono")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Tipo)
            .HasConversion(value => value.Value, value => TipoTelefono.Create(value))
            .HasColumnName("Tipo")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => new { t.ClienteId, t.Telefono })
            .IsUnique();

        builder.HasOne(t => t.Cliente)
            .WithMany(c => c.Telefonos)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

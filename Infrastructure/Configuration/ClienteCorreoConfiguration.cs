using Domain.Entities;
using Domain.ValueObjects.ClienteCorreos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class ClienteCorreoConfiguration : IEntityTypeConfiguration<ClienteCorreo>
{
    public void Configure(EntityTypeBuilder<ClienteCorreo> builder)
    {
        builder.ToTable("ClienteCorreos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Correo)
            .HasConversion(value => value.Value, value => CorreoElectronico.Create(value))
            .HasColumnName("Correo")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Principal)
            .HasColumnName("Principal")
            .IsRequired();

        builder.HasIndex(c => c.Correo)
            .IsUnique();

        builder.HasOne(c => c.Cliente)
            .WithMany(cliente => cliente.Correos)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

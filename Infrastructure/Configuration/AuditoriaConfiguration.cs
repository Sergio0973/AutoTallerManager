using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.ValueObjects.Auditorias;

namespace Infrastructure.Configuration;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("Auditorias");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Entidad)
            .HasConversion(
                v => v.Value,
                v => EntidadAuditada.Create(v));

        builder.Property(a => a.TipoAccion)
            .HasConversion(
                v => v.Value,
                v => TipoAccion.Create(v));

        builder.Property(a => a.DatosAnteriores)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? DatosJson.Create(v) : null);

        builder.Property(a => a.DatosNuevos)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? DatosJson.Create(v) : null);

        builder.Property(a => a.IpOrigen)
            .HasConversion(
                v => v.Value,
                v => IpOrigen.Create(v));

        builder.Property(a => a.Fecha).IsRequired();

        builder.HasOne(a => a.Usuario)
            .WithMany(u => u.Auditorias)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class OrdenMecanicoConfiguration : IEntityTypeConfiguration<OrdenMecanico>
{
    public void Configure(EntityTypeBuilder<OrdenMecanico> builder)
    {
        builder.ToTable("OrdenesMecanicos");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.FechaAsignacion)
            .HasColumnName("FechaAsignacion")
            .IsRequired();

        builder.HasIndex(o => new { o.OrdenId, o.MecanicoId })
            .IsUnique();

        builder.HasOne(o => o.Orden)
            .WithMany(orden => orden.Mecanicos)
            .HasForeignKey(o => o.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Mecanico)
            .WithMany(u => u.AsignacionesMecanico)
            .HasForeignKey(o => o.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

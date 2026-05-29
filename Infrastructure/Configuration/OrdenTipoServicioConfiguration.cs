using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class OrdenTipoServicioConfiguration : IEntityTypeConfiguration<OrdenTipoServicio>
{
    public void Configure(EntityTypeBuilder<OrdenTipoServicio> builder)
    {
        builder.ToTable("OrdenesTiposServicio");
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => new { o.OrdenId, o.TipoServicioId })
            .IsUnique();

        builder.HasOne(o => o.Orden)
            .WithMany(orden => orden.TiposServicio)
            .HasForeignKey(o => o.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.TipoServicio)
            .WithMany()
            .HasForeignKey(o => o.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

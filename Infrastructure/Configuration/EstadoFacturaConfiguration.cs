using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class EstadoFacturaConfiguration : IEntityTypeConfiguration<EstadoFactura>
{
    public void Configure(EntityTypeBuilder<EstadoFactura> builder)
    {
        builder.ToTable("EstadosFactura");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Nombre).IsUnique();
    }
}

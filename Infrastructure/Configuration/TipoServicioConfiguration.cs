using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class TipoServicioConfiguration : IEntityTypeConfiguration<TipoServicio>
{
    public void Configure(EntityTypeBuilder<TipoServicio> builder)
    {
        builder.ToTable("TiposServicio");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Nombre).IsUnique();
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class UnidadMedidaConfiguration : IEntityTypeConfiguration<UnidadMedida>
{
    public void Configure(EntityTypeBuilder<UnidadMedida> builder)
    {
        builder.ToTable("UnidadesMedida");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Nombre).IsUnique();
        builder.HasIndex(x => x.Abreviatura).IsUnique();
    }
}

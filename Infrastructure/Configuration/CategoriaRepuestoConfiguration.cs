using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class CategoriaRepuestoConfiguration : IEntityTypeConfiguration<CategoriaRepuesto>
{
    public void Configure(EntityTypeBuilder<CategoriaRepuesto> builder)
    {
        builder.ToTable("CategoriasRepuesto");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Nombre).IsUnique();
    }
}

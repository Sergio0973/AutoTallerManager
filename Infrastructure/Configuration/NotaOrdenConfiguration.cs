using Domain.Entities;
using Domain.ValueObjects.NotaOrdenes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class NotaOrdenConfiguration : IEntityTypeConfiguration<NotaOrden>
{
    public void Configure(EntityTypeBuilder<NotaOrden> builder)
    {
        builder.ToTable("NotasOrden");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Contenido)
            .HasConversion(value => value.Value, value => ContenidoNota.Create(value))
            .HasColumnName("Contenido")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(n => n.FechaNota)
            .HasColumnName("FechaNota")
            .IsRequired();

        builder.HasIndex(n => n.OrdenId);
        builder.HasIndex(n => n.UsuarioId);

        builder.HasOne(n => n.Orden)
            .WithMany(o => o.Notas)
            .HasForeignKey(n => n.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Usuario)
            .WithMany(u => u.Notas)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

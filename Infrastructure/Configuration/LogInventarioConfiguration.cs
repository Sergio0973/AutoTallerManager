using Domain.Entities;
using Domain.ValueObjects.LogInventarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class LogInventarioConfiguration : IEntityTypeConfiguration<LogInventario>
{
    public void Configure(EntityTypeBuilder<LogInventario> builder)
    {
        builder.ToTable("LogsInventario");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.TipoMovimiento)
            .HasConversion(value => value.Value, value => TipoMovimiento.Create(value))
            .HasColumnName("TipoMovimiento")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(l => l.Cantidad)
            .HasColumnName("Cantidad")
            .IsRequired();

        builder.Property(l => l.StockResultante)
            .HasColumnName("StockResultante")
            .IsRequired();

        builder.Property(l => l.Fecha)
            .HasColumnName("Fecha")
            .IsRequired();

        builder.Property(l => l.Motivo)
            .HasConversion(
                value => value != null ? value.Value : null,
                value => value != null ? MotivoMovimiento.Create(value) : null)
            .HasColumnName("Motivo")
            .HasMaxLength(500);

        builder.HasIndex(l => new { l.RepuestoId, l.Fecha });
        builder.HasIndex(l => l.UsuarioId);
        builder.HasIndex(l => l.OrdenId);
        builder.HasIndex(l => l.CompraId);

        builder.HasOne(l => l.Repuesto)
            .WithMany(r => r.LogsInventario)
            .HasForeignKey(l => l.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Usuario)
            .WithMany(u => u.LogsInventario)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Orden)
            .WithMany(o => o.LogsInventario)
            .HasForeignKey(l => l.OrdenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Compra)
            .WithMany(c => c.LogsInventario)
            .HasForeignKey(l => l.CompraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

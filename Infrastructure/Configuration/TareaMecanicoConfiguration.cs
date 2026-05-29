using Domain.Entities;
using Domain.ValueObjects.TareaMecanicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public sealed class TareaMecanicoConfiguration : IEntityTypeConfiguration<TareaMecanico>
{
    public void Configure(EntityTypeBuilder<TareaMecanico> builder)
    {
        builder.ToTable("TareasMecanicos");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Descripcion)
            .HasConversion(value => value.Value, value => DescripcionTarea.Create(value))
            .HasColumnName("Descripcion")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.HorasTrabajadas)
            .HasConversion(value => value.Value, value => HorasTrabajadas.Create(value))
            .HasColumnName("HorasTrabajadas")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(t => t.CostoHora)
            .HasConversion(value => value.Value, value => CostoHora.Create(value))
            .HasColumnName("CostoHora")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.Estado)
            .HasConversion(value => value.Value, value => EstadoTarea.Create(value))
            .HasColumnName("Estado")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.FechaInicio)
            .HasColumnName("FechaInicio");

        builder.Property(t => t.FechaFin)
            .HasColumnName("FechaFin");

        builder.HasIndex(t => t.OrdenId);
        builder.HasIndex(t => t.MecanicoId);
        builder.HasIndex(t => t.TipoServicioId);

        builder.HasOne(t => t.Orden)
            .WithMany(o => o.Tareas)
            .HasForeignKey(t => t.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Mecanico)
            .WithMany(u => u.Tareas)
            .HasForeignKey(t => t.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TipoServicio)
            .WithMany()
            .HasForeignKey(t => t.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

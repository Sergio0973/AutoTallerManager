using Domain.Entities;
using Domain.ValueObjects.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Correo)
            .HasConversion(
                correo => correo.Value,
                value => CorreoUsuario.Create(value))
            .HasColumnName("Correo")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Nombre)
            .HasConversion(
                nombre => nombre.Value,
                value => NombreUsuario.Create(value))
            .HasColumnName("Nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Activo).IsRequired();
        builder.Property(u => u.FechaCreacion).IsRequired();

        builder.HasIndex(u => u.Correo)
            .IsUnique();

        builder.HasMany(u => u.Auditorias)
            .WithOne(a => a.Usuario)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Citas)
            .WithOne(c => c.Recepcionista)
            .HasForeignKey(c => c.RecepcionistaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.OrdenesServicio)
            .WithOne(o => o.Recepcionista)
            .HasForeignKey(o => o.RecepcionistaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Compras)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AsignacionesMecanico)
            .WithOne(a => a.Mecanico)
            .HasForeignKey(a => a.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Tareas)
            .WithOne(t => t.Mecanico)
            .HasForeignKey(t => t.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notas)
            .WithOne(n => n.Usuario)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.HistorialEstados)
            .WithOne(h => h.Usuario)
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.LogsInventario)
            .WithOne(l => l.Usuario)
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.FacturasEmitidas)
            .WithOne(f => f.Usuario)
            .HasForeignKey(f => f.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.GarantiasMecanico)
            .WithOne(g => g.Mecanico)
            .HasForeignKey(g => g.MecanicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

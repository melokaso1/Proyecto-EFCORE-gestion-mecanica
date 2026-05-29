using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.IdUsuario);
        builder.HasIndex(u => u.IdPersona).IsUnique();
        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();

        builder.HasOne(u => u.Persona)
            .WithOne()
            .HasForeignKey<Usuario>(u => u.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioRoles"));

        builder.HasMany(u => u.Especializaciones)
            .WithMany(e => e.Mecanicos)
            .UsingEntity(j => j.ToTable("MecanicoEspecializaciones"));
    }
}

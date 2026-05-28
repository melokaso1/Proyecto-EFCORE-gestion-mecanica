using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CorreoPersonaConfiguration : IEntityTypeConfiguration<CorreoPersona>
{
    public void Configure(EntityTypeBuilder<CorreoPersona> builder)
    {
        builder.ToTable("CorreosPersona");
        builder.HasKey(c => c.IdCorreoPersona);
        builder.HasIndex(c => new { c.UsuarioCorreo, c.IdDominioCorreo }).IsUnique();
        builder.Property(c => c.UsuarioCorreo).HasMaxLength(100).IsRequired();
        builder.HasOne(c => c.DominioCorreo)
            .WithMany()
            .HasForeignKey(c => c.IdDominioCorreo)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Persona>()
            .WithMany(p => p.CorreosPersona)
            .HasForeignKey(c => c.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

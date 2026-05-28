using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TelefonoPersonaConfiguration : IEntityTypeConfiguration<TelefonoPersona>
{
    public void Configure(EntityTypeBuilder<TelefonoPersona> builder)
    {
        builder.ToTable("TelefonosPersona");
        builder.HasKey(t => t.IdTelefonoPersona);
        builder.HasIndex(t => new { t.IdCodigoTelefono, t.NumeroTelefono }).IsUnique();
        builder.Property(t => t.NumeroTelefono).HasMaxLength(30).IsRequired();
        builder.HasOne(t => t.CodigoTelefono)
            .WithMany()
            .HasForeignKey(t => t.IdCodigoTelefono)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Persona>()
            .WithMany(p => p.TelefonosPersona)
            .HasForeignKey(t => t.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("Personas");
        builder.HasKey(p => p.IdPersona);
        builder.Property(p => p.Nombres).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Apellidos).HasMaxLength(100).IsRequired();
        builder.Property(p => p.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

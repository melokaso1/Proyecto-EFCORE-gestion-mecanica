using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DocumentoPersonaConfiguration : IEntityTypeConfiguration<DocumentoPersona>
{
    public void Configure(EntityTypeBuilder<DocumentoPersona> builder)
    {
        builder.ToTable("DocumentosPersona");
        builder.HasKey(d => d.IdDocumentoPersona);
        builder.HasIndex(d => new { d.IdTipoDocumento, d.NumeroDocumento }).IsUnique();
        builder.Property(d => d.NumeroDocumento).HasMaxLength(50).IsRequired();
        builder.HasOne<Persona>()
            .WithMany()
            .HasForeignKey(d => d.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<TipoDocumento>()
            .WithMany()
            .HasForeignKey(d => d.IdTipoDocumento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

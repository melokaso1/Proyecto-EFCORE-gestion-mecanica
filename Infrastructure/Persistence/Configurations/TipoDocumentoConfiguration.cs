using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TipoDocumentoConfiguration : IEntityTypeConfiguration<TipoDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> builder)
    {
        builder.ToTable("TiposDocumento");
        builder.HasKey(t => t.IdTipoDocumento);
        builder.Property(t => t.Codigo).HasMaxLength(10).IsRequired();
        builder.HasIndex(t => t.Codigo).IsUnique();
        builder.Property(t => t.Nombre).HasMaxLength(80).IsRequired();
        builder.HasIndex(t => t.Nombre).IsUnique();
    }
}

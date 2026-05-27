using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TipoServicioConfiguration : IEntityTypeConfiguration<TipoServicio>
{
    public void Configure(EntityTypeBuilder<TipoServicio> builder)
    {
        builder.ToTable("TiposServicio");
        builder.HasKey(t => t.IdTipoServicio);
        builder.Property(t => t.Nombre).HasMaxLength(80).IsRequired();
        builder.HasIndex(t => t.Nombre).IsUnique();
    }
}

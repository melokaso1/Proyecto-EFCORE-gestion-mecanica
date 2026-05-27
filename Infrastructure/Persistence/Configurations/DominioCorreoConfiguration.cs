using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DominioCorreoConfiguration : IEntityTypeConfiguration<DominioCorreo>
{
    public void Configure(EntityTypeBuilder<DominioCorreo> builder)
    {
        builder.ToTable("DominiosCorreo");
        builder.HasKey(d => d.IdDominioCorreo);
        builder.Property(d => d.Dominio).HasMaxLength(100).IsRequired();
        builder.HasIndex(d => d.Dominio).IsUnique();
    }
}

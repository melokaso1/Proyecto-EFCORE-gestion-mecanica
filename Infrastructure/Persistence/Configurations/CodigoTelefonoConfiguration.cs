using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CodigoTelefonoConfiguration : IEntityTypeConfiguration<CodigoTelefono>
{
    public void Configure(EntityTypeBuilder<CodigoTelefono> builder)
    {
        builder.ToTable("CodigosTelefono");
        builder.HasKey(c => c.IdCodigoTelefono);
        builder.Property(c => c.Codigo).HasMaxLength(10).IsRequired();
        builder.HasIndex(c => c.Codigo).IsUnique();
        builder.Property(c => c.Pais).HasMaxLength(80).IsRequired();
    }
}

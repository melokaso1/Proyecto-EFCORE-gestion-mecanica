using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CategoriaRepuestoConfiguration : IEntityTypeConfiguration<CategoriaRepuesto>
{
    public void Configure(EntityTypeBuilder<CategoriaRepuesto> builder)
    {
        builder.ToTable("CategoriasRepuesto");
        builder.HasKey(c => c.IdCategoriaRepuesto);
        builder.Property(c => c.Nombre).HasMaxLength(80).IsRequired();
        builder.HasIndex(c => c.Nombre).IsUnique();
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.ToTable("Repuestos");
        builder.HasKey(r => r.IdRepuesto);
        builder.Property(r => r.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.Codigo).IsUnique();
        builder.Property(r => r.Descripcion).HasMaxLength(255).IsRequired();
        builder.Property(r => r.PrecioUnitario).HasPrecision(10, 2);
        builder.Property(r => r.Activo).HasDefaultValue(true);

        builder.HasOne(r => r.CategoriaRepuesto)
            .WithMany()
            .HasForeignKey(r => r.IdCategoriaRepuesto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

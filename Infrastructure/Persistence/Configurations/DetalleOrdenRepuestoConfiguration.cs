using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DetalleOrdenRepuestoConfiguration : IEntityTypeConfiguration<DetalleOrdenRepuesto>
{
    public void Configure(EntityTypeBuilder<DetalleOrdenRepuesto> builder)
    {
        builder.ToTable("DetalleOrdenRepuestos");
        builder.HasKey(d => d.IdDetalleOrdenRepuesto);
        builder.HasIndex(d => new { d.IdOrdenServicio, d.IdRepuesto }).IsUnique();
        builder.Property(d => d.PrecioUnitarioAplicado).HasPrecision(10, 2);

        builder.HasOne<OrdenServicio>()
            .WithMany()
            .HasForeignKey(d => d.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Repuesto>()
            .WithMany()
            .HasForeignKey(d => d.IdRepuesto)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

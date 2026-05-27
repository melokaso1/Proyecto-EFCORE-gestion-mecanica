using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Facturas");
        builder.HasKey(f => f.IdFactura);
        builder.HasIndex(f => f.IdOrdenServicio).IsUnique();
        builder.Property(f => f.ManoObra).HasPrecision(10, 2);
        builder.Property(f => f.Total).HasPrecision(10, 2);

        builder.HasOne<OrdenServicio>()
            .WithMany()
            .HasForeignKey(f => f.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DetalleFacturaConfiguration : IEntityTypeConfiguration<DetalleFactura>
{
    public void Configure(EntityTypeBuilder<DetalleFactura> builder)
    {
        builder.ToTable("DetalleFactura");
        builder.HasKey(d => d.IdDetalleFactura);
        builder.Property(d => d.Concepto).HasMaxLength(150).IsRequired();
        builder.Property(d => d.PrecioUnitario).HasPrecision(10, 2);

        builder.HasOne<Factura>()
            .WithMany(f => f.Detalles)
            .HasForeignKey(d => d.IdFactura)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos");
        builder.HasKey(p => p.IdPago);
        builder.Property(p => p.Metodo).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Referencia).HasMaxLength(80);

        builder.HasOne(p => p.OrdenServicio)
            .WithMany(o => o.Pagos)
            .HasForeignKey(p => p.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


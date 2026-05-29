using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.ToTable("Vehiculos");
        builder.HasKey(v => v.IdVehiculo);
        builder.Property(v => v.VIN).HasMaxLength(17).IsRequired();
        builder.Property(v => v.Placa).HasMaxLength(10).IsRequired();
        builder.HasIndex(v => v.VIN).IsUnique();
        builder.HasIndex(v => v.Placa).IsUnique();
        builder.HasOne(v => v.Modelo)
            .WithMany()
            .HasForeignKey(v => v.IdModelo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

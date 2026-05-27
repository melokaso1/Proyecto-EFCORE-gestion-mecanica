using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ModeloVehiculoConfiguration : IEntityTypeConfiguration<ModeloVehiculo>
{
    public void Configure(EntityTypeBuilder<ModeloVehiculo> builder)
    {
        builder.ToTable("ModelosVehiculo");
        builder.HasKey(m => m.IdModelo);
        builder.HasIndex(m => new { m.IdMarca, m.NombreModelo }).IsUnique();
        builder.Property(m => m.NombreModelo).HasMaxLength(80).IsRequired();
        builder.HasOne(m => m.Marca)
            .WithMany()
            .HasForeignKey(m => m.IdMarca)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MarcaVehiculoConfiguration : IEntityTypeConfiguration<MarcaVehiculo>
{
    public void Configure(EntityTypeBuilder<MarcaVehiculo> builder)
    {
        builder.ToTable("MarcasVehiculo");
        builder.HasKey(m => m.IdMarca);
        builder.Property(m => m.NombreMarca).HasMaxLength(80).IsRequired();
        builder.HasIndex(m => m.NombreMarca).IsUnique();
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TipoVehiculoConfiguration : IEntityTypeConfiguration<TipoVehiculo>
{
    public void Configure(EntityTypeBuilder<TipoVehiculo> builder)
    {
        builder.ToTable("TiposVehiculo");
        builder.HasKey(t => t.IdTipoVehiculo);
        builder.Property(t => t.Nombre).HasMaxLength(50).IsRequired();
        builder.HasIndex(t => t.Nombre).IsUnique();

        builder.HasData(
            new TipoVehiculo { IdTipoVehiculo = 1, Nombre = "Sedán" },
            new TipoVehiculo { IdTipoVehiculo = 2, Nombre = "Hatchback" },
            new TipoVehiculo { IdTipoVehiculo = 3, Nombre = "SUV" },
            new TipoVehiculo { IdTipoVehiculo = 4, Nombre = "Camioneta" },
            new TipoVehiculo { IdTipoVehiculo = 5, Nombre = "Pickup" },
            new TipoVehiculo { IdTipoVehiculo = 6, Nombre = "Motocicleta" }
        );
    }
}


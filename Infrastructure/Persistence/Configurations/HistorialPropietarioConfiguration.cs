using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class HistorialPropietarioConfiguration : IEntityTypeConfiguration<HistorialPropietarioVehiculo>
{
    public void Configure(EntityTypeBuilder<HistorialPropietarioVehiculo> builder)
    {
        builder.ToTable("HistorialPropietariosVehiculo");
        builder.HasKey(h => h.IdHistorialPropietario);
        builder.HasOne<Vehiculo>()
            .WithMany()
            .HasForeignKey(h => h.IdVehiculo)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(h => h.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

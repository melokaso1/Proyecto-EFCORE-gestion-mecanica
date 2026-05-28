using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrdenServicioConfiguration : IEntityTypeConfiguration<OrdenServicio>
{
    public void Configure(EntityTypeBuilder<OrdenServicio> builder)
    {
        builder.ToTable("OrdenesServicio");
        builder.HasKey(o => o.IdOrdenServicio);
        builder.Property(o => o.TrabajoRealizado).HasColumnType("text");

        builder.HasOne(o => o.Vehiculo)
            .WithMany()
            .HasForeignKey(o => o.IdVehiculo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Cliente)
            .WithMany()
            .HasForeignKey(o => o.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.TipoServicio)
            .WithMany()
            .HasForeignKey(o => o.IdTipoServicio)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Mecanico)
            .WithMany()
            .HasForeignKey(o => o.IdMecanico)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.EstadoOrden)
            .WithMany()
            .HasForeignKey(o => o.IdEstadoOrden)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

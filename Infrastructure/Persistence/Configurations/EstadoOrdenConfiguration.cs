using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EstadoOrdenConfiguration : IEntityTypeConfiguration<EstadoOrden>
{
    public void Configure(EntityTypeBuilder<EstadoOrden> builder)
    {
        builder.ToTable("EstadosOrden");
        builder.HasKey(e => e.IdEstadoOrden);
        builder.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.Nombre).IsUnique();

        builder.HasData(
            new EstadoOrden { IdEstadoOrden = 1, Nombre = EstadosOrden.Pendiente },
            new EstadoOrden { IdEstadoOrden = 2, Nombre = EstadosOrden.EnProceso },
            new EstadoOrden { IdEstadoOrden = 3, Nombre = EstadosOrden.Completada },
            new EstadoOrden { IdEstadoOrden = 4, Nombre = EstadosOrden.Cancelada });
    }
}

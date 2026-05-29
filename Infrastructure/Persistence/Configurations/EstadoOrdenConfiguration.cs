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
            new EstadoOrden { IdEstadoOrden = 4, Nombre = EstadosOrden.Cancelada },
            new EstadoOrden { IdEstadoOrden = 5, Nombre = EstadosOrden.Recibido },
            new EstadoOrden { IdEstadoOrden = 6, Nombre = EstadosOrden.DiagnosticoEnProceso },
            new EstadoOrden { IdEstadoOrden = 7, Nombre = EstadosOrden.PendienteAprobacionJefe },
            new EstadoOrden { IdEstadoOrden = 8, Nombre = EstadosOrden.PendienteAprobacionCliente },
            new EstadoOrden { IdEstadoOrden = 9, Nombre = EstadosOrden.RechazadoCliente },
            new EstadoOrden { IdEstadoOrden = 10, Nombre = EstadosOrden.AprobadoParcial },
            new EstadoOrden { IdEstadoOrden = 11, Nombre = EstadosOrden.AprobadoTotal },
            new EstadoOrden { IdEstadoOrden = 12, Nombre = EstadosOrden.ReparacionEnProceso },
            new EstadoOrden { IdEstadoOrden = 13, Nombre = EstadosOrden.ListoParaEntrega },
            new EstadoOrden { IdEstadoOrden = 14, Nombre = EstadosOrden.PendientePago },
            new EstadoOrden { IdEstadoOrden = 15, Nombre = EstadosOrden.Pagado },
            new EstadoOrden { IdEstadoOrden = 16, Nombre = EstadosOrden.Entregado },
            new EstadoOrden { IdEstadoOrden = 17, Nombre = EstadosOrden.Cerrado },
            new EstadoOrden { IdEstadoOrden = 18, Nombre = EstadosOrden.EnRegistro });
    }
}

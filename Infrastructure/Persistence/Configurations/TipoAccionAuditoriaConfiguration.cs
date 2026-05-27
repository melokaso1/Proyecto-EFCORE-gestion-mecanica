using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TipoAccionAuditoriaConfiguration : IEntityTypeConfiguration<TipoAccionAuditoria>
{
    public void Configure(EntityTypeBuilder<TipoAccionAuditoria> builder)
    {
        builder.ToTable("TiposAccionAuditoria");
        builder.HasKey(t => t.IdTipoAccionAuditoria);
        builder.Property(t => t.Nombre).HasMaxLength(50).IsRequired();
        builder.HasIndex(t => t.Nombre).IsUnique();

        builder.HasData(
            new TipoAccionAuditoria { IdTipoAccionAuditoria = 1, Nombre = "Creación" },
            new TipoAccionAuditoria { IdTipoAccionAuditoria = 2, Nombre = "Actualización" },
            new TipoAccionAuditoria { IdTipoAccionAuditoria = 3, Nombre = "Eliminación" });
    }
}

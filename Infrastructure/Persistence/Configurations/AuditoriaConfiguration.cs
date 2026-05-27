using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("Auditorias");
        builder.HasKey(a => a.IdAuditoria);
        builder.Property(a => a.EntidadAfectada).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Descripcion).HasColumnType("text");
        builder.Property(a => a.FechaHora).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(a => a.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TipoAccionAuditoria>()
            .WithMany()
            .HasForeignKey(a => a.IdTipoAccionAuditoria)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

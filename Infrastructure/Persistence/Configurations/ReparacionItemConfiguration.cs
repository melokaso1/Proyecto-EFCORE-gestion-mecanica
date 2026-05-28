using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReparacionItemConfiguration : IEntityTypeConfiguration<ReparacionItem>
{
    public void Configure(EntityTypeBuilder<ReparacionItem> builder)
    {
        builder.ToTable("ReparacionesItem");
        builder.HasKey(r => r.IdReparacionItem);

        builder.Property(r => r.Descripcion).HasMaxLength(300).IsRequired();
        builder.Property(r => r.Estado).HasMaxLength(60).IsRequired();

        builder.HasIndex(r => new { r.IdOrdenServicio, r.Orden }).IsUnique();

        builder.HasOne(r => r.OrdenServicio)
            .WithMany(o => o.Reparaciones)
            .HasForeignKey(r => r.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Mecanico)
            .WithMany()
            .HasForeignKey(r => r.IdMecanico)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


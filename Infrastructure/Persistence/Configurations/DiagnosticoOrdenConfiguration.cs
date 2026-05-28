using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DiagnosticoOrdenConfiguration : IEntityTypeConfiguration<DiagnosticoOrden>
{
    public void Configure(EntityTypeBuilder<DiagnosticoOrden> builder)
    {
        builder.ToTable("DiagnosticosOrden");
        builder.HasKey(d => d.IdDiagnosticoOrden);

        builder.Property(d => d.SintomasReportados).HasColumnType("text");
        builder.Property(d => d.Hallazgos).HasColumnType("text");
        builder.Property(d => d.Recomendaciones).HasColumnType("text");

        builder.HasIndex(d => d.IdOrdenServicio).IsUnique();

        builder.HasOne(d => d.OrdenServicio)
            .WithOne(o => o.Diagnostico)
            .HasForeignKey<DiagnosticoOrden>(d => d.IdOrdenServicio)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Mecanico)
            .WithMany()
            .HasForeignKey(d => d.IdMecanico)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


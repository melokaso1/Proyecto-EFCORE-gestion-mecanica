using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EspecializacionMecanicoConfiguration : IEntityTypeConfiguration<EspecializacionMecanico>
{
    public void Configure(EntityTypeBuilder<EspecializacionMecanico> builder)
    {
        builder.ToTable("EspecializacionesMecanico");
        builder.HasKey(e => e.IdEspecializacionMecanico);
        builder.Property(e => e.Codigo).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Nombre).HasMaxLength(80).IsRequired();
        builder.Property(e => e.Descripcion).HasMaxLength(250);
        builder.HasIndex(e => e.Codigo).IsUnique();

        builder.HasData(
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.DiagnosticoId,
                Codigo = EspecializacionesMecanico.Diagnostico,
                Nombre = "Diagnóstico",
                Descripcion = "Evaluación y detección de fallas",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.MotorId,
                Codigo = EspecializacionesMecanico.Motor,
                Nombre = "Motor",
                Descripcion = "Reparación de motor y componentes internos",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.FrenosId,
                Codigo = EspecializacionesMecanico.Frenos,
                Nombre = "Frenos",
                Descripcion = "Sistema de frenos y ABS",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.SuspensionId,
                Codigo = EspecializacionesMecanico.Suspension,
                Nombre = "Suspensión",
                Descripcion = "Suspensión, dirección y alineación",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.ElectricoId,
                Codigo = EspecializacionesMecanico.Electrico,
                Nombre = "Sistema eléctrico",
                Descripcion = "Batería, alternador, cableado y electrónica",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.TransmisionId,
                Codigo = EspecializacionesMecanico.Transmision,
                Nombre = "Transmisión",
                Descripcion = "Caja manual/automática y embrague",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.AireAcondicionadoId,
                Codigo = EspecializacionesMecanico.AireAcondicionado,
                Nombre = "Aire acondicionado",
                Descripcion = "Climatización y refrigeración",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.CarroceriaId,
                Codigo = EspecializacionesMecanico.Carroceria,
                Nombre = "Carrocería",
                Descripcion = "Latonería, pintura y estética",
                Activo = true
            },
            new EspecializacionMecanico
            {
                IdEspecializacionMecanico = EspecializacionesMecanico.MantenimientoGeneralId,
                Codigo = EspecializacionesMecanico.MantenimientoGeneral,
                Nombre = "Mantenimiento general",
                Descripcion = "Aceite, filtros y servicios preventivos",
                Activo = true
            });
    }
}

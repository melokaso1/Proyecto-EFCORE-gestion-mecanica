using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.IdRol);
        builder.Property(r => r.NombreRol).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.NombreRol).IsUnique();

        builder.HasData(
            new Rol { IdRol = 1, NombreRol = "Admin" },
            new Rol { IdRol = 2, NombreRol = "Mecánico" },
            new Rol { IdRol = 3, NombreRol = "Recepcionista" });
    }
}

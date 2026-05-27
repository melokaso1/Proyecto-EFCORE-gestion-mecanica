using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.IdCliente);
        builder.HasIndex(c => c.IdPersona).IsUnique();
        builder.HasOne(c => c.Persona)
            .WithOne()
            .HasForeignKey<Cliente>(c => c.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

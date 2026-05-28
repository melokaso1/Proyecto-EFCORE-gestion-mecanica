using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options) : DbContext(options)
{
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<DocumentoPersona> DocumentosPersona => Set<DocumentoPersona>();
    public DbSet<DominioCorreo> DominiosCorreo => Set<DominioCorreo>();
    public DbSet<CorreoPersona> CorreosPersona => Set<CorreoPersona>();
    public DbSet<CodigoTelefono> CodigosTelefono => Set<CodigoTelefono>();
    public DbSet<TelefonoPersona> TelefonosPersona => Set<TelefonoPersona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<TipoVehiculo> TiposVehiculo => Set<TipoVehiculo>();
    public DbSet<MarcaVehiculo> MarcasVehiculo => Set<MarcaVehiculo>();
    public DbSet<ModeloVehiculo> ModelosVehiculo => Set<ModeloVehiculo>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<HistorialPropietarioVehiculo> HistorialPropietariosVehiculo => Set<HistorialPropietarioVehiculo>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
    public DbSet<OrdenServicio> OrdenesServicio => Set<OrdenServicio>();
    public DbSet<DiagnosticoOrden> DiagnosticosOrden => Set<DiagnosticoOrden>();
    public DbSet<ReparacionItem> ReparacionesItem => Set<ReparacionItem>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<CategoriaRepuesto> CategoriasRepuesto => Set<CategoriaRepuesto>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<DetalleOrdenRepuesto> DetalleOrdenRepuestos => Set<DetalleOrdenRepuesto>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<DetalleFactura> DetalleFactura => Set<DetalleFactura>();
    public DbSet<TipoAccionAuditoria> TiposAccionAuditoria => Set<TipoAccionAuditoria>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutoTallerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

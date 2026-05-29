using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;

namespace Infrastructure.Persistence;

public class UnitOfWork(AutoTallerDbContext context) : IUnitOfWork, IDisposable
{
    private IClienteRepository? _clientes;
    private IVehiculoRepository? _vehiculos;
    private IOrdenServicioRepository? _ordenesServicio;
    private IRepuestoRepository? _repuestos;
    private IDetalleOrdenRepository? _detallesOrden;
    private IFacturaRepository? _facturas;
    private IUsuarioRepository? _usuarios;
    private IAuditoriaRepository? _auditorias;
    private IGenericRepository<Persona>? _personas;
    private IGenericRepository<DocumentoPersona>? _documentosPersona;
    private IGenericRepository<CorreoPersona>? _correosPersona;
    private IGenericRepository<TelefonoPersona>? _telefonosPersona;
    private IGenericRepository<DominioCorreo>? _dominiosCorreo;
    private IGenericRepository<CodigoTelefono>? _codigosTelefono;
    private IGenericRepository<HistorialPropietarioVehiculo>? _historialPropietarios;
    private IGenericRepository<EstadoOrden>? _estadosOrden;
    private IGenericRepository<TipoServicio>? _tiposServicio;
    private IGenericRepository<TipoAccionAuditoria>? _tiposAccionAuditoria;
    private IGenericRepository<Rol>? _roles;
    private IGenericRepository<CategoriaRepuesto>? _categoriasRepuesto;
    private IGenericRepository<DetalleFactura>? _detallesFactura;
    private IGenericRepository<DiagnosticoOrden>? _diagnosticosOrden;
    private IReparacionItemRepository? _reparacionesItem;
    private IGenericRepository<EspecializacionMecanico>? _especializacionesMecanico;
    private IGenericRepository<Pago>? _pagos;
    private IGenericRepository<TipoDocumento>? _tiposDocumento;

    public IClienteRepository Clientes => _clientes ??= new ClienteRepository(context);
    public IVehiculoRepository Vehiculos => _vehiculos ??= new VehiculoRepository(context);
    public IOrdenServicioRepository OrdenesServicio => _ordenesServicio ??= new OrdenServicioRepository(context);
    public IRepuestoRepository Repuestos => _repuestos ??= new RepuestoRepository(context);
    public IDetalleOrdenRepository DetallesOrden => _detallesOrden ??= new DetalleOrdenRepository(context);
    public IFacturaRepository Facturas => _facturas ??= new FacturaRepository(context);
    public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(context);
    public IAuditoriaRepository Auditorias => _auditorias ??= new AuditoriaRepository(context);
    public IGenericRepository<Persona> Personas => _personas ??= new GenericRepository<Persona>(context);
    public IGenericRepository<DocumentoPersona> DocumentosPersona => _documentosPersona ??= new GenericRepository<DocumentoPersona>(context);
    public IGenericRepository<CorreoPersona> CorreosPersona => _correosPersona ??= new GenericRepository<CorreoPersona>(context);
    public IGenericRepository<TelefonoPersona> TelefonosPersona => _telefonosPersona ??= new GenericRepository<TelefonoPersona>(context);
    public IGenericRepository<DominioCorreo> DominiosCorreo => _dominiosCorreo ??= new GenericRepository<DominioCorreo>(context);
    public IGenericRepository<CodigoTelefono> CodigosTelefono => _codigosTelefono ??= new GenericRepository<CodigoTelefono>(context);
    public IGenericRepository<HistorialPropietarioVehiculo> HistorialPropietarios => _historialPropietarios ??= new GenericRepository<HistorialPropietarioVehiculo>(context);
    public IGenericRepository<EstadoOrden> EstadosOrden => _estadosOrden ??= new GenericRepository<EstadoOrden>(context);
    public IGenericRepository<TipoServicio> TiposServicio => _tiposServicio ??= new GenericRepository<TipoServicio>(context);
    public IGenericRepository<TipoAccionAuditoria> TiposAccionAuditoria => _tiposAccionAuditoria ??= new GenericRepository<TipoAccionAuditoria>(context);
    public IGenericRepository<Rol> Roles => _roles ??= new GenericRepository<Rol>(context);
    public IGenericRepository<CategoriaRepuesto> CategoriasRepuesto => _categoriasRepuesto ??= new GenericRepository<CategoriaRepuesto>(context);
    public IGenericRepository<DetalleFactura> DetallesFactura => _detallesFactura ??= new GenericRepository<DetalleFactura>(context);
    public IGenericRepository<DiagnosticoOrden> DiagnosticosOrden => _diagnosticosOrden ??= new GenericRepository<DiagnosticoOrden>(context);
    public IReparacionItemRepository ReparacionesItem => _reparacionesItem ??= new ReparacionItemRepository(context);
    public IGenericRepository<EspecializacionMecanico> EspecializacionesMecanico => _especializacionesMecanico ??= new GenericRepository<EspecializacionMecanico>(context);
    public IGenericRepository<Pago> Pagos => _pagos ??= new GenericRepository<Pago>(context);
    public IGenericRepository<TipoDocumento> TiposDocumento => _tiposDocumento ??= new GenericRepository<TipoDocumento>(context);

    public Task<int> CommitAsync() => context.SaveChangesAsync();

    public void Dispose() => context.Dispose();
}

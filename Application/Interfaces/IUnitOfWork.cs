using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IUnitOfWork
{
    IClienteRepository Clientes { get; }
    IVehiculoRepository Vehiculos { get; }
    IOrdenServicioRepository OrdenesServicio { get; }
    IRepuestoRepository Repuestos { get; }
    IDetalleOrdenRepository DetallesOrden { get; }
    IFacturaRepository Facturas { get; }
    IUsuarioRepository Usuarios { get; }
    IAuditoriaRepository Auditorias { get; }
    IGenericRepository<Persona> Personas { get; }
    IGenericRepository<DocumentoPersona> DocumentosPersona { get; }
    IGenericRepository<CorreoPersona> CorreosPersona { get; }
    IGenericRepository<TelefonoPersona> TelefonosPersona { get; }
    IGenericRepository<DominioCorreo> DominiosCorreo { get; }
    IGenericRepository<CodigoTelefono> CodigosTelefono { get; }
    IGenericRepository<HistorialPropietarioVehiculo> HistorialPropietarios { get; }
    IGenericRepository<EstadoOrden> EstadosOrden { get; }
    IGenericRepository<TipoServicio> TiposServicio { get; }
    IGenericRepository<TipoAccionAuditoria> TiposAccionAuditoria { get; }
    IGenericRepository<Rol> Roles { get; }
    IGenericRepository<CategoriaRepuesto> CategoriasRepuesto { get; }
    IGenericRepository<DetalleFactura> DetallesFactura { get; }
    IGenericRepository<DiagnosticoOrden> DiagnosticosOrden { get; }
    IReparacionItemRepository ReparacionesItem { get; }
    IGenericRepository<EspecializacionMecanico> EspecializacionesMecanico { get; }
    IGenericRepository<Pago> Pagos { get; }
    IGenericRepository<TipoDocumento> TiposDocumento { get; }
    Task<int> CommitAsync();
}

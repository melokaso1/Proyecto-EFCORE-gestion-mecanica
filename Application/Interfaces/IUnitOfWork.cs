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
    Task<int> CommitAsync();
}

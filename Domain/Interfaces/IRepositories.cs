using Domain.Entities;

namespace Domain.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<bool> ExisteConOrdenesActivasAsync(int idCliente);
    Task<Cliente?> ObtenerPorDocumentoAsync(string numeroDocumento);
}

public interface IVehiculoRepository : IGenericRepository<Vehiculo>
{
    Task<Vehiculo?> ObtenerPorVinAsync(string vin);
    Task<Vehiculo?> ObtenerPorPlacaAsync(string placa);
    Task<bool> ExisteConOrdenesActivasAsync(int idVehiculo);
    Task<IReadOnlyList<ModeloVehiculo>> ListarModelosCatalogoAsync();
}

public interface IOrdenServicioRepository : IGenericRepository<OrdenServicio>
{
    Task<(IEnumerable<OrdenServicio> items, int total)> GetPagedConFiltrosAsync(
        int pageNumber,
        int pageSize,
        string? estado,
        int? idCliente,
        int? idMecanico,
        DateTime? desde,
        DateTime? hasta);
}

public interface IRepuestoRepository : IGenericRepository<Repuesto>
{
    Task<(IEnumerable<Repuesto> items, int total)> ListarConFiltrosAsync(
        int pageNumber,
        int pageSize,
        string? descripcion,
        int? idCategoria,
        bool? soloConStockMinimo);
}

public interface IDetalleOrdenRepository : IGenericRepository<DetalleOrdenRepuesto>;

public interface IFacturaRepository : IGenericRepository<Factura>;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<bool> ExisteConRolAsync(string nombreRol);
    Task<(IEnumerable<Usuario> items, int total)> GetEmpleadosPagedAsync(int pageNumber, int pageSize);
    Task<bool> TieneEspecializacionAsync(int idUsuario, int idEspecializacion);
    Task<bool> TieneEspecializacionPorCodigoAsync(int idUsuario, string codigo);
}

public interface IAuditoriaRepository : IGenericRepository<Auditoria>;

public interface IReparacionItemRepository : IGenericRepository<ReparacionItem>
{
    Task<IReadOnlyList<ReparacionItem>> ListarPorOrdenConDetalleAsync(int idOrdenServicio);
    Task<IReadOnlyList<ReparacionItem>> ListarPendientesJefeConDetalleAsync(int page, int size);
    Task<ReparacionItem?> GetDetalleAsync(int idReparacionItem);
}

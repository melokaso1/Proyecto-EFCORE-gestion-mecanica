using Domain.Entities;

namespace Domain.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>
{
    Task<bool> ExisteConOrdenesActivasAsync(int idCliente);
}

public interface IVehiculoRepository : IGenericRepository<Vehiculo>
{
    Task<Vehiculo?> ObtenerPorVinAsync(string vin);
    Task<bool> ExisteConOrdenesActivasAsync(int idVehiculo);
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

public interface IUsuarioRepository : IGenericRepository<Usuario>;

public interface IAuditoriaRepository : IGenericRepository<Auditoria>;

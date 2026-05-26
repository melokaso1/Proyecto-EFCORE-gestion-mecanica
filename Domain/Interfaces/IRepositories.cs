using Domain.Entities;

namespace Domain.Interfaces;

public interface IClienteRepository : IGenericRepository<Cliente>;

public interface IVehiculoRepository : IGenericRepository<Vehiculo>;

public interface IOrdenServicioRepository : IGenericRepository<OrdenServicio>;

public interface IRepuestoRepository : IGenericRepository<Repuesto>;

public interface IDetalleOrdenRepository : IGenericRepository<DetalleOrdenRepuesto>;

public interface IFacturaRepository : IGenericRepository<Factura>;

public interface IUsuarioRepository : IGenericRepository<Usuario>;

public interface IAuditoriaRepository : IGenericRepository<Auditoria>;

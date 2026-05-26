using Domain.Constants;

namespace Domain.Entities;

public class OrdenServicio
{
    public int IdOrdenServicio { get; set; }
    public int IdVehiculo { get; set; }
    public int IdTipoServicio { get; set; }
    public int IdMecanico { get; set; }
    public int IdEstadoOrden { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
    public string? TrabajoRealizado { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public TipoServicio? TipoServicio { get; set; }
    public Usuario? Mecanico { get; set; }
    public EstadoOrden? EstadoOrden { get; set; }

    public bool EstaActiva()
    {
        if (EstadoOrden is null)
            return false;

        return EstadoOrden.Nombre is not EstadosOrden.Completada
            and not EstadosOrden.Cancelada;
    }
}

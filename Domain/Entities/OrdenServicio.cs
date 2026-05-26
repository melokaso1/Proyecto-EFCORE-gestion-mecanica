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
}

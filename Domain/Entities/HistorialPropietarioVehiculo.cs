namespace Domain.Entities;

public class HistorialPropietarioVehiculo
{
    public int IdHistorialPropietario { get; set; }
    public int IdVehiculo { get; set; }
    public int IdCliente { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

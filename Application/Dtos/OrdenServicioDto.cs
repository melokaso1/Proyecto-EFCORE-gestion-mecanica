namespace Application.Dtos;

public class OrdenServicioDto
{
    public int IdOrdenServicio { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string MarcaModelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public string MecanicoNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
    public string? TrabajoRealizado { get; set; }
}

public class CreateOrdenServicioDto
{
    public int IdCliente { get; set; }
    public int IdVehiculo { get; set; }
    public int IdTipoServicio { get; set; }
    public int IdMecanico { get; set; }
}

public class UpdateOrdenServicioDto
{
    public int IdEstadoOrden { get; set; }
    public string? TrabajoRealizado { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
}

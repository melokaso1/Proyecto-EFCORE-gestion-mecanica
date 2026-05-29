namespace Frontend.Models;

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

public class PagedOrdenesResult
{
    public List<OrdenServicioDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

public class SeguimientoOrdenDto
{
    public int IdOrdenServicio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string MarcaModelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string TipoServicio { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
    public string? TrabajoRealizado { get; set; }
}

public class AdminDashboardDto
{
    public int TotalUsuarios { get; set; }
    public int UsuariosPendientesRol { get; set; }
    public int TotalClientes { get; set; }
    public int TotalVehiculos { get; set; }
    public int TotalEmpleados { get; set; }
    public Dictionary<string, int> OrdenesPorEstado { get; set; } = [];
}

public class RecepcionDashboardDto
{
    public int TotalClientes { get; set; }
    public int TotalVehiculos { get; set; }
    public int OrdenesHoy { get; set; }
    public List<OrdenServicioDto> OrdenesRecientes { get; set; } = [];
}

namespace Frontend.Models;

public class ClientePortalOrden
{
    public int IdOrdenServicio { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string MarcaModelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string TipoServicio { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
    public string? TrabajoRealizado { get; set; }

    public decimal? CostoPropuesto { get; set; }
    public string? NotaCostoPropuesto { get; set; }
    public bool? CostoAprobado { get; set; }
    public DateTime? FechaDecisionCosto { get; set; }
    public string? ComentarioCliente { get; set; }

    public decimal? TotalFactura { get; set; }
}

public class ClienteDecisionCostoRequest
{
    public bool Aprobado { get; set; }
    public string? Comentario { get; set; }
}


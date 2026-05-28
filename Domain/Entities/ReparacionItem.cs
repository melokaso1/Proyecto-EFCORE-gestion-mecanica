using Domain.Constants;

namespace Domain.Entities;

public class ReparacionItem
{
    public int IdReparacionItem { get; set; }
    public int IdOrdenServicio { get; set; }
    public int? IdMecanico { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public decimal? HorasManoObraEstimada { get; set; }

    public string Estado { get; set; } = EstadosReparacionItem.Propuesto;
    public bool? AprobadoPorJefe { get; set; }
    public DateTime? FechaDecisionJefe { get; set; }
    public string? ComentarioJefe { get; set; }

    public bool? AprobadoPorCliente { get; set; }
    public DateTime? FechaDecisionCliente { get; set; }
    public string? ComentarioCliente { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public OrdenServicio? OrdenServicio { get; set; }
    public Usuario? Mecanico { get; set; }
}


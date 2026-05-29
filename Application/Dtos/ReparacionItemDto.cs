namespace Application.Dtos;

public class ReparacionItemDto
{
    public int IdReparacionItem { get; set; }
    public int IdOrdenServicio { get; set; }
    public int Orden { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public decimal? HorasManoObraEstimada { get; set; }
    public int IdEspecializacionMecanico { get; set; }
    public string EspecializacionNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    public bool? AprobadoPorJefe { get; set; }
    public DateTime? FechaDecisionJefe { get; set; }
    public string? ComentarioJefe { get; set; }

    public bool? AprobadoPorCliente { get; set; }
    public DateTime? FechaDecisionCliente { get; set; }
    public string? ComentarioCliente { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}

public class CreateReparacionItemDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public decimal? HorasManoObraEstimada { get; set; }
    public int IdEspecializacionMecanico { get; set; }
}

public class UpdateReparacionItemDto
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal CostoEstimado { get; set; }
    public decimal? HorasManoObraEstimada { get; set; }
    public int IdEspecializacionMecanico { get; set; }
}

public class DecisionJefeDto
{
    public bool Aprobar { get; set; }
    public string? Comentario { get; set; }
}

public class DecisionClienteDto
{
    public bool Aprobar { get; set; }
    public string? Comentario { get; set; }
}


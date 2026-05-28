namespace Domain.Entities;

public class DiagnosticoOrden
{
    public int IdDiagnosticoOrden { get; set; }
    public int IdOrdenServicio { get; set; }
    public int IdMecanico { get; set; }
    public DateTime FechaDiagnostico { get; set; }
    public string? SintomasReportados { get; set; }
    public string? Hallazgos { get; set; }
    public string? Recomendaciones { get; set; }

    public OrdenServicio? OrdenServicio { get; set; }
    public Usuario? Mecanico { get; set; }
}


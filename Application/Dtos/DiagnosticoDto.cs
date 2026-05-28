namespace Application.Dtos;

public class DiagnosticoDto
{
    public int IdDiagnosticoOrden { get; set; }
    public int IdOrdenServicio { get; set; }
    public int IdMecanico { get; set; }
    public DateTime FechaDiagnostico { get; set; }
    public string? SintomasReportados { get; set; }
    public string? Hallazgos { get; set; }
    public string? Recomendaciones { get; set; }
}

public class UpsertDiagnosticoDto
{
    public string? SintomasReportados { get; set; }
    public string? Hallazgos { get; set; }
    public string? Recomendaciones { get; set; }
}


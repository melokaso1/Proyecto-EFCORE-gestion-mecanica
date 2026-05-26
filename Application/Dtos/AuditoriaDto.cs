namespace Application.Dtos;

public class AuditoriaDto
{
    public int IdAuditoria { get; set; }
    public int IdUsuario { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
    public string EntidadAfectada { get; set; } = string.Empty;
    public int IdRegistroAfectado { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Descripcion { get; set; }
}

namespace Domain.Entities;

public class Auditoria
{
    public int IdAuditoria { get; set; }
    public int IdUsuario { get; set; }
    public int IdTipoAccionAuditoria { get; set; }
    public string EntidadAfectada { get; set; } = string.Empty;
    public int IdRegistroAfectado { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Descripcion { get; set; }
}

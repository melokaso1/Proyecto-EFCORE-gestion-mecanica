namespace Domain.Entities;

public class EspecializacionMecanico
{
    public int IdEspecializacionMecanico { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Usuario> Mecanicos { get; set; } = [];
    public ICollection<ReparacionItem> Reparaciones { get; set; } = [];
}

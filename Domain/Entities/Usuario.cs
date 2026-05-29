namespace Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public int IdPersona { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool Estado { get; set; }
    public Persona? Persona { get; set; }
    public ICollection<Rol> Roles { get; set; } = [];
    public ICollection<EspecializacionMecanico> Especializaciones { get; set; } = [];
}

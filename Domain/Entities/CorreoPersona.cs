namespace Domain.Entities;

public class CorreoPersona
{
    public int IdCorreoPersona { get; set; }
    public int IdPersona { get; set; }
    public int IdDominioCorreo { get; set; }
    public string UsuarioCorreo { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
}

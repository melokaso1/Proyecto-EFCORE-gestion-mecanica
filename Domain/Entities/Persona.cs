namespace Domain.Entities;

public class Persona
{
    public int IdPersona { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public ICollection<CorreoPersona> CorreosPersona { get; set; } = [];
    public ICollection<TelefonoPersona> TelefonosPersona { get; set; } = [];
}

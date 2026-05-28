namespace Domain.Entities;

public class TelefonoPersona
{
    public int IdTelefonoPersona { get; set; }
    public int IdPersona { get; set; }
    public int IdCodigoTelefono { get; set; }
    public string NumeroTelefono { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
    public CodigoTelefono? CodigoTelefono { get; set; }
}

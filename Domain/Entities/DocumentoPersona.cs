namespace Domain.Entities;

public class DocumentoPersona
{
    public int IdDocumentoPersona { get; set; }
    public int IdPersona { get; set; }
    public int IdTipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
}

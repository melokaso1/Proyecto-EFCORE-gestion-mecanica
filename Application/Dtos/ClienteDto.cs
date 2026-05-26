namespace Application.Dtos;

public class ClienteDto
{
    public int IdCliente { get; set; }
    public PersonaDto Persona { get; set; } = null!;
    public bool Estado { get; set; }
}

public class CreateClienteDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int IdTipoDocumento { get; set; }
}

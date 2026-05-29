namespace Frontend.Models;

public class ClienteDto
{
    public int IdCliente { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string CorreoPrincipal { get; set; } = string.Empty;
    public string TelefonoPrincipal { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public int IdTipoDocumento { get; set; }
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

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

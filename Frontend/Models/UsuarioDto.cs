namespace Frontend.Models;

public class UsuarioDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public List<EspecializacionMecanicoDto> Especializaciones { get; set; } = [];
}

public class EspecializacionMecanicoDto
{
    public int IdEspecializacionMecanico { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class PagedUsuariosResult
{
    public List<UsuarioDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
}

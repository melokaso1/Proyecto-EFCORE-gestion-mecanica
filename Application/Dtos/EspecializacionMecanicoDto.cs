namespace Application.Dtos;

public class EspecializacionMecanicoDto
{
    public int IdEspecializacionMecanico { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class AsignarEspecializacionesDto
{
    public List<int> IdsEspecializaciones { get; set; } = [];
}

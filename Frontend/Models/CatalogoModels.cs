namespace Frontend.Models;

public class MarcaVehiculoDto
{
    public int IdMarca { get; set; }
    public string NombreMarca { get; set; } = string.Empty;
}

public class CreateMarcaVehiculoDto
{
    public string NombreMarca { get; set; } = string.Empty;
}

public class CreateModeloVehiculoDto
{
    public int IdMarca { get; set; }
    public int IdTipoVehiculo { get; set; }
    public string NombreModelo { get; set; } = string.Empty;
}

public class TipoVehiculoDto
{
    public int IdTipoVehiculo { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

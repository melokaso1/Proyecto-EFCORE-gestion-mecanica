namespace Frontend.Models;

public class VehiculoListDto
{
    public int IdVehiculo { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
}

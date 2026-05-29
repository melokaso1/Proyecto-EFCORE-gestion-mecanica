namespace Application.Dtos;

public class VehiculoDto
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

public class CreateVehiculoDto
{
    public int IdModelo { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
    public int IdCliente { get; set; }
}

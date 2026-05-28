namespace Domain.Entities;

public class ModeloVehiculo
{
    public int IdModelo { get; set; }
    public int IdMarca { get; set; }
    public int IdTipoVehiculo { get; set; }
    public string NombreModelo { get; set; } = string.Empty;
    public MarcaVehiculo? Marca { get; set; }
    public TipoVehiculo? TipoVehiculo { get; set; }
}

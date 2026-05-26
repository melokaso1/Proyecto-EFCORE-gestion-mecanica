namespace Domain.Entities;

public class Vehiculo
{
    public int IdVehiculo { get; set; }
    public int IdModelo { get; set; }
    public string VIN { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Kilometraje { get; set; }
}

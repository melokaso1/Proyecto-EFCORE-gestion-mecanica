namespace Domain.Entities;

public class DetalleOrdenRepuesto
{
    public int IdDetalleOrdenRepuesto { get; set; }
    public int IdOrdenServicio { get; set; }
    public int IdRepuesto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioAplicado { get; set; }
}

namespace Application.Dtos;

public class DetalleOrdenDto
{
    public int IdRepuesto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioAplicado { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitarioAplicado;
}

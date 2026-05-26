namespace Domain.Entities;

public class DetalleFactura
{
    public int IdDetalleFactura { get; set; }
    public int IdFactura { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

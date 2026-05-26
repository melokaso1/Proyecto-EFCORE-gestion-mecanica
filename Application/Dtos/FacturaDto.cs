namespace Application.Dtos;

public class FacturaDto
{
    public int IdFactura { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaFactura { get; set; }
    public decimal ManoObra { get; set; }
    public List<DetalleFacturaDto> Detalles { get; set; } = [];
    public decimal Total { get; set; }
}

public class DetalleFacturaDto
{
    public string Concepto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

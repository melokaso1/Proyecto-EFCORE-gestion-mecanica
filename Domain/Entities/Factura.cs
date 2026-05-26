namespace Domain.Entities;

public class Factura
{
    public int IdFactura { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaFactura { get; set; }
    public decimal ManoObra { get; set; }
    public decimal Total { get; set; }
    public ICollection<DetalleFactura> Detalles { get; set; } = [];
}

namespace Application.Dtos;

public class PagoDto
{
    public int IdPago { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaPago { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
}

public class RegistrarPagoDto
{
    public int IdOrdenServicio { get; set; }
    public string Metodo { get; set; } = string.Empty; // efectivo/transferencia/tarjeta
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
    public decimal ManoObra { get; set; } = 0m;
}

public class PagoYFacturaDto
{
    public PagoDto Pago { get; set; } = new();
    public FacturaDto? Factura { get; set; }
}


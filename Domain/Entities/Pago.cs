namespace Domain.Entities;

public class Pago
{
    public int IdPago { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaPago { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }

    public OrdenServicio? OrdenServicio { get; set; }
}


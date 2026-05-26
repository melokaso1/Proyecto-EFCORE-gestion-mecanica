namespace Domain.Entities;

public class Repuesto
{
    public int IdRepuesto { get; set; }
    public int IdCategoriaRepuesto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioUnitario { get; set; }
    public bool Activo { get; set; }
}

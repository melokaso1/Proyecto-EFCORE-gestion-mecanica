namespace Application.Dtos;

public class RepuestoDto
{
    public int IdRepuesto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class CreateRepuestoDto
{
    public int IdCategoriaRepuesto { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public decimal PrecioUnitario { get; set; }
}

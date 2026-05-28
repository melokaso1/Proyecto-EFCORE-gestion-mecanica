using System.Net.Http.Json;
using System.Text.Json;

namespace Frontend.Services;

public interface ICajaClientService
{
    Task<PagoYFacturaDto> RegistrarPagoAsync(RegistrarPagoDto dto);
}

public class CajaClientService(HttpClient http, AuthService auth) : ICajaClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagoYFacturaDto> RegistrarPagoAsync(RegistrarPagoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/caja/pagos");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PagoYFacturaDto>(JsonOptions))!;
    }
}

public class RegistrarPagoDto
{
    public int IdOrdenServicio { get; set; }
    public string Metodo { get; set; } = "efectivo";
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
    public decimal ManoObra { get; set; }
}

public class PagoDto
{
    public int IdPago { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaPago { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Referencia { get; set; }
}

public class FacturaDto
{
    public int IdFactura { get; set; }
    public int IdOrdenServicio { get; set; }
    public DateTime FechaFactura { get; set; }
    public decimal ManoObra { get; set; }
    public decimal Total { get; set; }
    public List<DetalleFacturaDto> Detalles { get; set; } = [];
}

public class DetalleFacturaDto
{
    public int IdDetalleFactura { get; set; }
    public int IdFactura { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class PagoYFacturaDto
{
    public PagoDto Pago { get; set; } = new();
    public FacturaDto? Factura { get; set; }
}


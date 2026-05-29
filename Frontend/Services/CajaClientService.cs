using System.Net.Http.Json;
using System.Text.Json;

namespace Frontend.Services;

public interface ICajaClientService
{
    Task<IReadOnlyList<OrdenPendientePagoDto>> ListarOrdenesPendientesAsync();
    Task<IReadOnlyList<PagoDto>> ListarPagosRecientesAsync(int limit = 20);
    Task<PagoYFacturaDto> RegistrarPagoAsync(RegistrarPagoDto dto);
}

public class CajaClientService(HttpClient http, AuthService auth) : ICajaClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<OrdenPendientePagoDto>> ListarOrdenesPendientesAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/caja/ordenes-pendientes");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<List<OrdenPendientePagoDto>>(JsonOptions) ?? [];
    }

    public async Task<IReadOnlyList<PagoDto>> ListarPagosRecientesAsync(int limit = 20)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/caja/pagos?limit={limit}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<List<PagoDto>>(JsonOptions) ?? [];
    }

    public async Task<PagoYFacturaDto> RegistrarPagoAsync(RegistrarPagoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/caja/pagos");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<PagoYFacturaDto>(JsonOptions))!;
    }

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        string message;
        try
        {
            using var doc = JsonDocument.Parse(body);
            message = doc.RootElement.TryGetProperty("message", out var m)
                ? m.GetString() ?? response.ReasonPhrase ?? "Error"
                : response.ReasonPhrase ?? "Error";
        }
        catch
        {
            message = response.ReasonPhrase ?? "Error";
        }

        throw new InvalidOperationException(message);
    }
}

public class OrdenPendientePagoDto
{
    public int IdOrdenServicio { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public decimal TotalEstimado { get; set; }
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

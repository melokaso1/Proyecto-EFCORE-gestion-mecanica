using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public class VehiculosService(HttpClient http, AuthService auth) : IVehiculosService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedResult<VehiculoListDto>> GetVehiculosAsync(
        int page, int size, string? vinFiltro, string? placaFiltro = null)
    {
        var url = $"api/vehiculos?pageNumber={page}&pageSize={size}";
        if (!string.IsNullOrWhiteSpace(vinFiltro))
            url += $"&vin={Uri.EscapeDataString(vinFiltro.Trim())}";
        if (!string.IsNullOrWhiteSpace(placaFiltro))
            url += $"&placa={Uri.EscapeDataString(placaFiltro.Trim())}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<VehiculoListDto>>(JsonOptions) ?? [];

        var totalCount = 0;
        if (response.Headers.TryGetValues("X-Total-Count", out var values))
            int.TryParse(values.FirstOrDefault(), out totalCount);

        return new PagedResult<VehiculoListDto>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<VehiculoListDto> RegistrarEnCatalogoAsync(CreateVehiculoCasoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/vehiculos/catalogo");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<VehiculoListDto>(JsonOptions))!;
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

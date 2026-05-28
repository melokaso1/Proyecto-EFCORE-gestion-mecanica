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

    public async Task<PagedResult<VehiculoListDto>> GetVehiculosAsync(int page, int size, string? vinFiltro)
    {
        var url = $"api/vehiculos?pageNumber={page}&pageSize={size}";
        if (!string.IsNullOrWhiteSpace(vinFiltro))
            url += $"&vin={Uri.EscapeDataString(vinFiltro.Trim())}";

        // #region agent log
        Frontend.DebugSessionLogger.Log(
            location: "Frontend/Services/VehiculosService.cs:GetVehiculosAsync:request",
            message: "Requesting vehiculos",
            data: new
            {
                baseAddress = http.BaseAddress?.ToString(),
                url,
                page,
                size,
                vinLen = vinFiltro?.Length
            },
            hypothesisId: "H5-frontend-api-base-url-or-auth",
            runId: "pre-fix");
        // #endregion agent log

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);

        // #region agent log
        Frontend.DebugSessionLogger.Log(
            location: "Frontend/Services/VehiculosService.cs:GetVehiculosAsync:response",
            message: "Vehiculos response received",
            data: new
            {
                statusCode = (int)response.StatusCode,
                isSuccess = response.IsSuccessStatusCode,
                hasTotalHeader = response.Headers.Contains("X-Total-Count")
            },
            hypothesisId: "H5-frontend-api-base-url-or-auth",
            runId: "pre-fix");
        // #endregion agent log

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
}

using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public class OrdenesService(HttpClient http, AuthService auth) : IOrdenesService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<PagedOrdenesResult> GetMisOrdenesAsync(int page = 1, int size = 20, string? estado = null) =>
        GetPagedAsync($"api/ordenesservicio/mis-ordenes?pageNumber={page}&pageSize={size}" +
                      (estado is null ? "" : $"&estado={Uri.EscapeDataString(estado)}"));

    public Task<PagedOrdenesResult> GetOrdenesAsync(int page = 1, int size = 20, string? estado = null) =>
        GetPagedAsync($"api/ordenesservicio?pageNumber={page}&pageSize={size}" +
                      (estado is null ? "" : $"&estado={Uri.EscapeDataString(estado)}"));

    private async Task<PagedOrdenesResult> GetPagedAsync(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<OrdenServicioDto>>(JsonOptions) ?? [];

        var totalCount = 0;
        if (response.Headers.TryGetValues("X-Total-Count", out var values))
            int.TryParse(values.FirstOrDefault(), out totalCount);

        return new PagedOrdenesResult { Items = items, TotalCount = totalCount };
    }
}

public class SeguimientoService(HttpClient http) : ISeguimientoService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<(SeguimientoOrdenDto? Resultado, string? Error)> ConsultarAsync(
        string documento, string? vin, int? codigoOrden)
    {
        try
        {
            var query = $"api/seguimiento?documento={Uri.EscapeDataString(documento.Trim())}";
            if (!string.IsNullOrWhiteSpace(vin))
                query += $"&vin={Uri.EscapeDataString(vin.Trim())}";
            if (codigoOrden is not null)
                query += $"&codigoOrden={codigoOrden}";

            using var response = await http.GetAsync(query);
            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<SeguimientoOrdenDto>(JsonOptions);
                return (resultado, null);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return (null, "No se encontró información con esos datos. Verifica documento y VIN o número de orden.");

            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
            return (null, error?.Message ?? "No se pudo consultar el seguimiento.");
        }
        catch
        {
            return (null, "No se pudo conectar con la API. ¿Está ejecutándose el backend?");
        }
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}

public class DashboardService(HttpClient http, AuthService auth) : IDashboardService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AdminDashboardDto?> GetAdminAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/dashboard/admin");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AdminDashboardDto>(JsonOptions)
            : null;
    }

    public async Task<RecepcionDashboardDto?> GetRecepcionAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/dashboard/recepcion");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<RecepcionDashboardDto>(JsonOptions)
            : null;
    }
}

public class EmpleadosService(HttpClient http, AuthService auth) : IEmpleadosService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedUsuariosResult> GetEmpleadosAsync(int page = 1, int size = 50)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/usuarios/empleados?pageNumber={page}&pageSize={size}");
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<UsuarioDto>>(JsonOptions) ?? [];

        var totalCount = 0;
        if (response.Headers.TryGetValues("X-Total-Count", out var values))
            int.TryParse(values.FirstOrDefault(), out totalCount);

        return new PagedUsuariosResult { Items = items, TotalCount = totalCount };
    }
}

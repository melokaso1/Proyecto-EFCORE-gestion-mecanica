using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public class ClientePortalService(HttpClient http, AuthService auth) : IClientePortalService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<PagedClientePortalOrdenesResult> GetMisOrdenesAsync(int page = 1, int size = 20, string? estado = null)
    {
        var url = $"api/portalcliente/ordenes?pageNumber={page}&pageSize={size}" +
                  (estado is null ? "" : $"&estado={Uri.EscapeDataString(estado)}");

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<ClientePortalOrden>>(JsonOptions) ?? [];

        var totalCount = 0;
        if (response.Headers.TryGetValues("X-Total-Count", out var values))
            int.TryParse(values.FirstOrDefault(), out totalCount);

        return new PagedClientePortalOrdenesResult { Items = items, TotalCount = totalCount };
    }

    public async Task DecidirCostoAsync(int idOrdenServicio, bool aprobado, string? comentario)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"api/portalcliente/ordenes/{idOrdenServicio}/costo/decision");

        request.Content = JsonContent.Create(new ClienteDecisionCostoRequest
        {
            Aprobado = aprobado,
            Comentario = comentario
        });

        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ReparacionItemDto>> GetReparacionesOrdenAsync(int idOrdenServicio)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/portalcliente/ordenes/{idOrdenServicio}/reparaciones");
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ReparacionItemDto>>(JsonOptions) ?? [];
    }
}

public class PagedClientePortalOrdenesResult
{
    public List<ClientePortalOrden> Items { get; set; } = [];
    public int TotalCount { get; set; }
}


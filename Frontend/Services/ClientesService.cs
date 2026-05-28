using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Frontend.Models;

namespace Frontend.Services;

public class ClientesService(HttpClient http, AuthService auth) : IClientesService
{
    public async Task<PagedResult<ClienteDto>> GetClientesAsync(int page, int size, string? filtro)
    {
        var url = $"api/clientes?pageNumber={page}&pageSize={size}";
        if (!string.IsNullOrWhiteSpace(filtro))
            url += $"&filtro={Uri.EscapeDataString(filtro)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var apiItems = await response.Content.ReadFromJsonAsync<List<ApiClienteDto>>()
            ?? [];

        var totalCount = 0;
        if (response.Headers.TryGetValues("X-Total-Count", out var values))
            int.TryParse(values.FirstOrDefault(), out totalCount);

        return new PagedResult<ClienteDto>
        {
            Items = apiItems.Select(MapToClienteDto).ToList(),
            TotalCount = totalCount
        };
    }

    public async Task CreateClienteAsync(CreateClienteDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/clientes")
        {
            Content = JsonContent.Create(dto)
        };
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
    }

    public async Task UpdateClienteAsync(int id, CreateClienteDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/clientes/{id}")
        {
            Content = JsonContent.Create(dto)
        };
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
    }

    public async Task DeleteClienteAsync(int id)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/clientes/{id}");
        auth.ApplyAuthorization(request);

        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
    }

    private static ClienteDto MapToClienteDto(ApiClienteDto api) => new()
    {
        IdCliente = api.IdCliente,
        Nombres = api.Persona?.Nombres ?? string.Empty,
        Apellidos = api.Persona?.Apellidos ?? string.Empty,
        CorreoPrincipal = api.CorreoPrincipal ?? string.Empty,
        TelefonoPrincipal = api.TelefonoPrincipal ?? string.Empty,
        Estado = api.Estado
    };

    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(body)
                ? $"Error HTTP {(int)response.StatusCode}"
                : body);
    }

    private sealed class ApiClienteDto
    {
        [JsonPropertyName("idCliente")]
        public int IdCliente { get; set; }

        [JsonPropertyName("persona")]
        public ApiPersonaDto? Persona { get; set; }

        [JsonPropertyName("correoPrincipal")]
        public string? CorreoPrincipal { get; set; }

        [JsonPropertyName("telefonoPrincipal")]
        public string? TelefonoPrincipal { get; set; }

        [JsonPropertyName("estado")]
        public bool Estado { get; set; }
    }

    private sealed class ApiPersonaDto
    {
        [JsonPropertyName("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [JsonPropertyName("apellidos")]
        public string Apellidos { get; set; } = string.Empty;
    }
}

using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IDiagnosticoClientService
{
    Task<DiagnosticoDto?> GetDiagnosticoAsync(int idOrdenServicio);
    Task<DiagnosticoDto> UpsertDiagnosticoAsync(int idOrdenServicio, UpsertDiagnosticoDto dto);
    Task<List<ReparacionItemDto>> GetReparacionesAsync(int idOrdenServicio);
    Task<ReparacionItemDto> CrearReparacionAsync(int idOrdenServicio, CreateReparacionItemDto dto);
    Task<ReparacionItemDto> DecidirJefeAsync(int idOrdenServicio, int idReparacionItem, bool aprobar, string? comentario);
    Task<List<ReparacionItemDto>> PendientesJefeAsync(int page = 1, int size = 50);
    Task<List<ReparacionItemDto>> DecidirClienteAsync(int idOrdenServicio, Dictionary<int, DecisionClienteDto> decisiones);
    Task<ReparacionItemDto> IniciarReparacionAsync(int idOrdenServicio, int idReparacionItem);
    Task<ReparacionItemDto> TerminarReparacionAsync(int idOrdenServicio, int idReparacionItem);
}

public class DiagnosticoClientService(HttpClient http, AuthService auth) : IDiagnosticoClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<DiagnosticoDto?> GetDiagnosticoAsync(int idOrdenServicio)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/ordenesservicio/{idOrdenServicio}/diagnostico");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<DiagnosticoDto>(JsonOptions)
            : null;
    }

    public async Task<DiagnosticoDto> UpsertDiagnosticoAsync(int idOrdenServicio, UpsertDiagnosticoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/ordenesservicio/{idOrdenServicio}/diagnostico");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DiagnosticoDto>(JsonOptions))!;
    }

    public async Task<List<ReparacionItemDto>> GetReparacionesAsync(int idOrdenServicio)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/ordenesservicio/{idOrdenServicio}/reparaciones");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ReparacionItemDto>>(JsonOptions) ?? [];
    }

    public async Task<ReparacionItemDto> CrearReparacionAsync(int idOrdenServicio, CreateReparacionItemDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/ordenesservicio/{idOrdenServicio}/reparaciones");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ReparacionItemDto>(JsonOptions))!;
    }

    public async Task<ReparacionItemDto> DecidirJefeAsync(int idOrdenServicio, int idReparacionItem, bool aprobar, string? comentario)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch,
            $"api/ordenesservicio/{idOrdenServicio}/reparaciones/{idReparacionItem}/decision-jefe");
        request.Content = JsonContent.Create(new DecisionJefeDto { Aprobar = aprobar, Comentario = comentario });
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ReparacionItemDto>(JsonOptions))!;
    }

    public async Task<List<ReparacionItemDto>> PendientesJefeAsync(int page = 1, int size = 50)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/jefe-mecanicos/pendientes?pageNumber={page}&pageSize={size}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ReparacionItemDto>>(JsonOptions) ?? [];
    }

    private sealed class ClienteDecisionesRequest
    {
        public Dictionary<int, DecisionClienteDto> Decisiones { get; set; } = new();
    }

    public async Task<List<ReparacionItemDto>> DecidirClienteAsync(int idOrdenServicio, Dictionary<int, DecisionClienteDto> decisiones)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/ordenesservicio/{idOrdenServicio}/reparaciones/decision-cliente");
        request.Content = JsonContent.Create(new ClienteDecisionesRequest { Decisiones = decisiones });
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ReparacionItemDto>>(JsonOptions) ?? [];
    }

    public async Task<ReparacionItemDto> IniciarReparacionAsync(int idOrdenServicio, int idReparacionItem)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch,
            $"api/ordenesservicio/{idOrdenServicio}/reparaciones/{idReparacionItem}/iniciar");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ReparacionItemDto>(JsonOptions))!;
    }

    public async Task<ReparacionItemDto> TerminarReparacionAsync(int idOrdenServicio, int idReparacionItem)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch,
            $"api/ordenesservicio/{idOrdenServicio}/reparaciones/{idReparacionItem}/terminar");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ReparacionItemDto>(JsonOptions))!;
    }
}


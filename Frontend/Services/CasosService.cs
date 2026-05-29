using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface ICasosService
{
    Task<CasoRecepcionDto> IniciarAsync();
    Task<List<CasoRecepcionDto>> ListarEnRegistroAsync(int page = 1, int size = 20);
    Task<CasoRecepcionDto?> ObtenerAsync(int id);
    Task<ClienteBusquedaDto?> BuscarClienteAsync(string documento);
    Task<VehiculoBusquedaDto?> BuscarVehiculoPorPlacaAsync(string placa);
    Task<VehiculoBusquedaDto?> BuscarVehiculoPorVinAsync(string vin);
    Task<CasoRecepcionDto> AsignarClienteAsync(int id, AsignarClienteCasoDto dto);
    Task<CasoRecepcionDto> AsignarVehiculoAsync(int id, AsignarVehiculoCasoDto dto);
    Task<CasoRecepcionDto> ConfirmarAsync(int id, ConfirmarCasoDto dto);
    Task CancelarAsync(int id);
    Task<List<TipoServicioDto>> TiposServicioAsync();
    Task<List<ModeloVehiculoCatalogoDto>> ModelosVehiculoAsync();
    Task<List<TipoDocumentoDto>> TiposDocumentoAsync();
}

public class CasosService(HttpClient http, AuthService auth) : ICasosService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<CasoRecepcionDto> IniciarAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/casos");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CasoRecepcionDto>(JsonOptions))!;
    }

    public async Task<List<CasoRecepcionDto>> ListarEnRegistroAsync(int page = 1, int size = 20)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/casos?pageNumber={page}&pageSize={size}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<CasoRecepcionDto>>(JsonOptions) ?? [];
    }

    public async Task<CasoRecepcionDto?> ObtenerAsync(int id)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/casos/{id}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CasoRecepcionDto>(JsonOptions)
            : null;
    }

    public async Task<ClienteBusquedaDto?> BuscarClienteAsync(string documento)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/casos/buscar/cliente?documento={Uri.EscapeDataString(documento)}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        var persona = root.GetProperty("persona");
        return new ClienteBusquedaDto
        {
            IdCliente = root.GetProperty("idCliente").GetInt32(),
            NombreCompleto = $"{persona.GetProperty("nombres").GetString()} {persona.GetProperty("apellidos").GetString()}".Trim(),
            CorreoPrincipal = root.TryGetProperty("correoPrincipal", out var c) ? c.GetString() ?? "" : "",
            TelefonoPrincipal = root.TryGetProperty("telefonoPrincipal", out var t) ? t.GetString() ?? "" : ""
        };
    }

    public async Task<VehiculoBusquedaDto?> BuscarVehiculoPorPlacaAsync(string placa)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/casos/buscar/vehiculo?placa={Uri.EscapeDataString(placa)}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<VehiculoBusquedaDto>(JsonOptions)
            : null;
    }

    public async Task<VehiculoBusquedaDto?> BuscarVehiculoPorVinAsync(string vin)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"api/casos/buscar/vehiculo?vin={Uri.EscapeDataString(vin)}");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<VehiculoBusquedaDto>(JsonOptions)
            : null;
    }

    public async Task<CasoRecepcionDto> AsignarClienteAsync(int id, AsignarClienteCasoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/casos/{id}/cliente");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<CasoRecepcionDto>(JsonOptions))!;
    }

    public async Task<CasoRecepcionDto> AsignarVehiculoAsync(int id, AsignarVehiculoCasoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/casos/{id}/vehiculo");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<CasoRecepcionDto>(JsonOptions))!;
    }

    public async Task<CasoRecepcionDto> ConfirmarAsync(int id, ConfirmarCasoDto dto)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/casos/{id}/confirmar");
        request.Content = JsonContent.Create(dto);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<CasoRecepcionDto>(JsonOptions))!;
    }

    public async Task CancelarAsync(int id)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/casos/{id}/cancelar");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<TipoServicioDto>> TiposServicioAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/catalogos/tipos-servicio");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TipoServicioDto>>(JsonOptions) ?? [];
    }

    public async Task<List<ModeloVehiculoCatalogoDto>> ModelosVehiculoAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/catalogos/modelos-vehiculo");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ModeloVehiculoCatalogoDto>>(JsonOptions) ?? [];
    }

    public async Task<List<TipoDocumentoDto>> TiposDocumentoAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/catalogos/tipos-documento");
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<TipoDocumentoDto>>(JsonOptions) ?? [];
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

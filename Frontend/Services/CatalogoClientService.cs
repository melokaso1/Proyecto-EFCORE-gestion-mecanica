using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface ICatalogoClientService
{
    Task<List<MarcaVehiculoDto>> ListarMarcasAsync();
    Task<List<ModeloVehiculoCatalogoDto>> ListarModelosAsync();
    Task<List<TipoVehiculoDto>> ListarTiposVehiculoAsync();
    Task<MarcaVehiculoDto> CrearMarcaAsync(CreateMarcaVehiculoDto dto);
    Task<MarcaVehiculoDto> ActualizarMarcaAsync(int id, CreateMarcaVehiculoDto dto);
    Task EliminarMarcaAsync(int id);
    Task<ModeloVehiculoCatalogoDto> CrearModeloAsync(CreateModeloVehiculoDto dto);
    Task<ModeloVehiculoCatalogoDto> ActualizarModeloAsync(int id, CreateModeloVehiculoDto dto);
    Task EliminarModeloAsync(int id);
}

public class CatalogoClientService(HttpClient http, AuthService auth) : ICatalogoClientService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<MarcaVehiculoDto>> ListarMarcasAsync() =>
        await GetAsync<List<MarcaVehiculoDto>>("api/catalogos/marcas-vehiculo") ?? [];

    public async Task<List<ModeloVehiculoCatalogoDto>> ListarModelosAsync() =>
        await GetAsync<List<ModeloVehiculoCatalogoDto>>("api/catalogos/modelos-vehiculo") ?? [];

    public async Task<List<TipoVehiculoDto>> ListarTiposVehiculoAsync() =>
        await GetAsync<List<TipoVehiculoDto>>("api/catalogos/tipos-vehiculo") ?? [];

    public async Task<MarcaVehiculoDto> CrearMarcaAsync(CreateMarcaVehiculoDto dto) =>
        await SendAsync<MarcaVehiculoDto>(HttpMethod.Post, "api/catalogos/marcas-vehiculo", dto);

    public async Task<MarcaVehiculoDto> ActualizarMarcaAsync(int id, CreateMarcaVehiculoDto dto) =>
        await SendAsync<MarcaVehiculoDto>(HttpMethod.Put, $"api/catalogos/marcas-vehiculo/{id}", dto);

    public async Task EliminarMarcaAsync(int id) =>
        await SendAsync(HttpMethod.Delete, $"api/catalogos/marcas-vehiculo/{id}");

    public async Task<ModeloVehiculoCatalogoDto> CrearModeloAsync(CreateModeloVehiculoDto dto) =>
        await SendAsync<ModeloVehiculoCatalogoDto>(HttpMethod.Post, "api/catalogos/modelos-vehiculo", dto);

    public async Task<ModeloVehiculoCatalogoDto> ActualizarModeloAsync(int id, CreateModeloVehiculoDto dto) =>
        await SendAsync<ModeloVehiculoCatalogoDto>(HttpMethod.Put, $"api/catalogos/modelos-vehiculo/{id}", dto);

    public async Task EliminarModeloAsync(int id) =>
        await SendAsync(HttpMethod.Delete, $"api/catalogos/modelos-vehiculo/{id}");

    private async Task<T?> GetAsync<T>(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    private async Task SendAsync(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
            request.Content = JsonContent.Create(body);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
            request.Content = JsonContent.Create(body);
        auth.ApplyAuthorization(request);
        using var response = await http.SendAsync(request);
        await EnsureSuccessOrThrowAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
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

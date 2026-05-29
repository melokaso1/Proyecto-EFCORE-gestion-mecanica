using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

public class UsuariosService(HttpClient http, AuthService auth) : IUsuariosService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<bool> IsAdminRegistrationAvailableAsync()
    {
        try
        {
            var response = await http.GetFromJsonAsync<AdminDisponibleResponse>(
                "api/usuarios/registro/admin-disponible", JsonOptions);
            return response?.Disponible ?? false;
        }
        catch
        {
            return false;
        }
    }

    public Task<(bool Success, string? Error)> RegisterAdminAsync(RegisterUserDto dto) =>
        PostRegistrationAsync("api/usuarios/registro/admin", dto, sendAuth: true);

    public Task<(bool Success, string? Error)> RegisterUsuarioAsync(RegisterUserDto dto) =>
        PostRegistrationAsync("api/usuarios/registro/usuario", dto, sendAuth: true);

    public async Task<PagedUsuariosResult> GetUsuariosAsync(int page, int size)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/usuarios?pageNumber={page}&pageSize={size}");
            auth.ApplyAuthorization(request);

            using var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return new PagedUsuariosResult();

            var items = await response.Content.ReadFromJsonAsync<List<UsuarioDto>>(JsonOptions) ?? [];

            var totalCount = 0;
            if (response.Headers.TryGetValues("X-Total-Count", out var values))
                int.TryParse(values.FirstOrDefault(), out totalCount);

            return new PagedUsuariosResult { Items = items, TotalCount = totalCount };
        }
        catch
        {
            return new PagedUsuariosResult();
        }
    }

    public async Task<(bool Success, string? Error)> AsignarRolAsync(int idUsuario, int idRol)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/usuarios/{idUsuario}/rol")
            {
                Content = JsonContent.Create(new { IdRol = idRol })
            };
            auth.ApplyAuthorization(request);

            using var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ReadErrorMessageAsync(response);
            return (false, error);
        }
        catch
        {
            return (false, "No se pudo conectar con la API.");
        }
    }

    public async Task<List<EspecializacionMecanicoDto>> GetEspecializacionesAsync()
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/especializaciones-mecanico");
            auth.ApplyAuthorization(request);
            using var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadFromJsonAsync<List<EspecializacionMecanicoDto>>(JsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task<(bool Success, string? Error)> EliminarUsuarioAsync(int idUsuario)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/usuarios/{idUsuario}");
            auth.ApplyAuthorization(request);
            using var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ReadErrorMessageAsync(response);
            return (false, error);
        }
        catch
        {
            return (false, "No se pudo conectar con la API.");
        }
    }

    public async Task<(bool Success, string? Error)> AsignarEspecializacionesAsync(
        int idUsuario,
        IReadOnlyList<int> idsEspecializaciones)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Patch, $"api/usuarios/{idUsuario}/especializaciones")
            {
                Content = JsonContent.Create(new { IdsEspecializaciones = idsEspecializaciones })
            };
            auth.ApplyAuthorization(request);
            using var response = await http.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return (true, null);
            var error = await ReadErrorMessageAsync(response);
            return (false, error);
        }
        catch
        {
            return (false, "No se pudo conectar con la API.");
        }
    }

    private async Task<(bool Success, string? Error)> PostRegistrationAsync(
        string url,
        RegisterUserDto dto,
        bool sendAuth)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(new
                {
                    dto.Nombres,
                    dto.Apellidos,
                    dto.Correo,
                    dto.Password
                })
            };

            if (sendAuth)
                auth.ApplyAuthorization(request);

            using var response = await http.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return (true, null);

            var error = await ReadErrorMessageAsync(response);
            return (false, error);
        }
        catch
        {
            return (false, "No se pudo conectar con la API. ¿Está ejecutándose el backend?");
        }
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
            if (!string.IsNullOrWhiteSpace(payload?.Message))
                return payload.Message;
        }
        catch
        {
            // Ignorar parseo de error
        }

        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.Conflict => "Ya existe un usuario con ese correo.",
            System.Net.HttpStatusCode.BadRequest => "Datos inválidos. Revisa el formulario.",
            System.Net.HttpStatusCode.Unauthorized => "Debes iniciar sesión como administrador para registrar otro admin.",
            _ => "No se pudo completar la operación. Intenta de nuevo."
        };
    }

    private sealed class AdminDisponibleResponse
    {
        public bool Disponible { get; set; }
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}

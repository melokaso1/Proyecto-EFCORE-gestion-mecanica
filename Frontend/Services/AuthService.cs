using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;
using Microsoft.JSInterop;

namespace Frontend.Services;

public class AuthService(HttpClient http, IJSRuntime js)
{
    private const string StorageKey = "mj_auth";
    private bool _initialized;

    public string? Token { get; private set; }
    public string? Correo { get; private set; }
    public DateTime? Expiracion { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token) &&
        Expiracion.HasValue &&
        Expiracion.Value.ToUniversalTime() > DateTime.UtcNow;

    public event Action? AuthStateChanged;

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;

        try
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var stored = JsonSerializer.Deserialize<StoredAuth>(json);
            if (stored is null || string.IsNullOrWhiteSpace(stored.Token))
                return;

            if (stored.Expiracion.ToUniversalTime() <= DateTime.UtcNow)
            {
                await ClearStorageAsync();
                return;
            }

            Token = stored.Token;
            Correo = stored.Correo;
            Expiracion = stored.Expiracion;
        }
        catch
        {
            await ClearStorageAsync();
        }
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string correo, string password)
    {
        try
        {
            // #region agent log
            Frontend.DebugSessionLogger.Log(
                location: "Frontend/Services/AuthService.cs:LoginAsync:enter",
                message: "Login attempt started",
                data: new
                {
                    correoLen = correo?.Length,
                    baseAddress = http.BaseAddress?.ToString()
                },
                hypothesisId: "H5-frontend-api-base-url-or-auth",
                runId: "pre-fix");
            // #endregion agent log

            var response = await http.PostAsJsonAsync("api/usuarios/login", new LoginRequest
            {
                Correo = correo,
                Password = password
            });

            // #region agent log
            Frontend.DebugSessionLogger.Log(
                location: "Frontend/Services/AuthService.cs:LoginAsync:response",
                message: "Login response received",
                data: new
                {
                    statusCode = (int)response.StatusCode,
                    isSuccess = response.IsSuccessStatusCode
                },
                hypothesisId: "H5-frontend-api-base-url-or-auth",
                runId: "pre-fix");
            // #endregion agent log

            if (!response.IsSuccessStatusCode)
            {
                var error = await ReadLoginErrorAsync(response);
                return (false, error);
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.Token))
                return (false, "No se recibió un token válido del servidor.");

            Token = tokenResponse.Token;
            Correo = correo;
            Expiracion = tokenResponse.Expiracion;

            // #region agent log
            Frontend.DebugSessionLogger.Log(
                location: "Frontend/Services/AuthService.cs:LoginAsync:success",
                message: "Login success; auth state updated",
                data: new
                {
                    hasExp = Expiracion.HasValue,
                    expUtc = Expiracion?.ToUniversalTime().ToString("O")
                },
                hypothesisId: "H5-frontend-api-base-url-or-auth",
                runId: "pre-fix");
            // #endregion agent log

            await js.InvokeVoidAsync("localStorage.setItem", StorageKey,
                JsonSerializer.Serialize(new StoredAuth
                {
                    Token = Token,
                    Correo = correo,
                    Expiracion = tokenResponse.Expiracion
                }));

            AuthStateChanged?.Invoke();
            return (true, null);
        }
        catch
        {
            return (false, "No se pudo conectar con la API. ¿Está ejecutándose el backend?");
        }
    }

    public async Task LogoutAsync()
    {
        await ClearStorageAsync();
        AuthStateChanged?.Invoke();
    }

    public void ApplyAuthorization(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(Token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
    }

    private async Task ClearStorageAsync()
    {
        Token = null;
        Correo = null;
        Expiracion = null;

        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch
        {
            // Ignorar errores de JS interop al cerrar sesión
        }
    }

    private static async Task<string> ReadLoginErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            if (!string.IsNullOrWhiteSpace(payload?.Message))
                return payload.Message;
        }
        catch
        {
            // Ignorar parseo
        }

        return "Credenciales inválidas. Verifica tu correo y contraseña.";
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}

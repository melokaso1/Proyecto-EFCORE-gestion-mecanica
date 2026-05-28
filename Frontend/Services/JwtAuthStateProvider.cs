using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _auth;

    public JwtAuthStateProvider(AuthService auth)
    {
        _auth = auth;
        _auth.AuthStateChanged += () =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _auth.InitializeAsync();

        if (!_auth.IsAuthenticated || string.IsNullOrWhiteSpace(_auth.Token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseJwtClaims(_auth.Token);
        if (!string.IsNullOrWhiteSpace(_auth.Correo))
            claims.Add(new Claim(ClaimTypes.Email, _auth.Correo));

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static List<Claim> ParseJwtClaims(string jwt)
    {
        var claims = new List<Claim>();
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return claims;

        try
        {
            var payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            payload = payload.Replace('-', '+').Replace('_', '/');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var doc = JsonDocument.Parse(json);

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                        AddClaim(claims, prop.Name, item.GetString());
                }
                else
                {
                    AddClaim(claims, prop.Name, prop.Value.GetString());
                }
            }
        }
        catch
        {
            // Token malformado
        }

        return claims;
    }

    private static void AddClaim(List<Claim> claims, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        claims.Add(name switch
        {
            "role" => new Claim(ClaimTypes.Role, value),
            "email" => new Claim(ClaimTypes.Email, value),
            "sub" => new Claim(ClaimTypes.NameIdentifier, value),
            _ when name.EndsWith("/role", StringComparison.OrdinalIgnoreCase) => new Claim(ClaimTypes.Role, value),
            _ when name.EndsWith("/emailaddress", StringComparison.OrdinalIgnoreCase) => new Claim(ClaimTypes.Email, value),
            _ => new Claim(name, value)
        });
    }
}

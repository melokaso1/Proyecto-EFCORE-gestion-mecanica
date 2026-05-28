namespace Frontend.Models;

public class LoginRequest
{
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}

public class StoredAuth
{
    public string Token { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}

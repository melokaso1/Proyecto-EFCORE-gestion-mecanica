using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class JwtTokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    public TokenResponseDto Generate(Usuario usuario, string email)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurada.");
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer no configurada.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience no configurada.");
        var expiresMinutes = int.Parse(configuration["Jwt:ExpiresInMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.Email, email)
        };

        foreach (var rol in usuario.Roles)
            claims.Add(new Claim(ClaimTypes.Role, rol.NombreRol));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiration,
            signingCredentials: credentials);

        return new TokenResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracion = expiration
        };
    }
}

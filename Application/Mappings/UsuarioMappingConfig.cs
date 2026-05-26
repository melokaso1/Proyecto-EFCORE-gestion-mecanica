using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class UsuarioMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Usuario, UsuarioDto>()
            .Map(dest => dest.NombreCompleto, src =>
                src.Persona == null
                    ? string.Empty
                    : $"{src.Persona.Nombres} {src.Persona.Apellidos}".Trim())
            .Map(dest => dest.Correo, src => ObtenerCorreoPrincipal(src))
            .Map(dest => dest.Roles, src => src.Roles.Select(r => r.NombreRol).ToList())
            .IgnoreNonMapped(true);
    }

    private static string ObtenerCorreoPrincipal(Usuario usuario)
    {
        var correo = usuario.Persona?.CorreosPersona
            .FirstOrDefault(c => c.EsPrincipal);

        return correo?.UsuarioCorreo ?? string.Empty;
    }
}

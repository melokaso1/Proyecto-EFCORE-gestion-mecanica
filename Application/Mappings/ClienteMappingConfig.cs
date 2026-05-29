using Application.Dtos;
using Domain.Entities;
using Mapster;
using System.Linq;

namespace Application.Mappings;

public class ClienteMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Persona, PersonaDto>();

        config.NewConfig<Cliente, ClienteDto>()
            .Map(dest => dest.Persona, src => src.Persona)
            .Map(dest => dest.CorreoPrincipal, src => BuildCorreoPrincipal(src))
            .Map(dest => dest.TelefonoPrincipal, src => BuildTelefonoPrincipal(src))
            .Map(dest => dest.NumeroDocumento, _ => string.Empty)
            .Map(dest => dest.IdTipoDocumento, _ => 0);

        config.NewConfig<CreateClienteDto, Persona>()
            .Map(dest => dest.Nombres, src => src.Nombres)
            .Map(dest => dest.Apellidos, src => src.Apellidos)
            .Map(dest => dest.FechaRegistro, _ => DateTime.UtcNow);

        config.NewConfig<CreateClienteDto, Cliente>()
            .Map(dest => dest.Estado, _ => true);
    }

    private static string BuildCorreoPrincipal(Cliente src)
    {
        if (src.Persona?.CorreosPersona is null || src.Persona.CorreosPersona.Count == 0)
            return string.Empty;

        var cp = src.Persona.CorreosPersona.FirstOrDefault(c => c.EsPrincipal)
                 ?? src.Persona.CorreosPersona.First();
        var dom = cp.DominioCorreo?.Dominio;
        return string.IsNullOrEmpty(dom) ? cp.UsuarioCorreo : $"{cp.UsuarioCorreo}@{dom}";
    }

    private static string BuildTelefonoPrincipal(Cliente src)
    {
        if (src.Persona?.TelefonosPersona is null || src.Persona.TelefonosPersona.Count == 0)
            return string.Empty;

        var tp = src.Persona.TelefonosPersona.FirstOrDefault(t => t.EsPrincipal)
                 ?? src.Persona.TelefonosPersona.First();
        var cod = tp.CodigoTelefono?.Codigo?.Trim() ?? string.Empty;
        var num = tp.NumeroTelefono?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(cod))
            return num;
        return $"{cod} {num}".Trim();
    }
}

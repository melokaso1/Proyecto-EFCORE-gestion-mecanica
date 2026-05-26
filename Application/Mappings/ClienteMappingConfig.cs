using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class ClienteMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Persona, PersonaDto>();

        config.NewConfig<Cliente, ClienteDto>()
            .Map(dest => dest.Persona, src => src.Persona);

        config.NewConfig<CreateClienteDto, Persona>()
            .Map(dest => dest.Nombres, src => src.Nombres)
            .Map(dest => dest.Apellidos, src => src.Apellidos)
            .Map(dest => dest.FechaRegistro, _ => DateTime.UtcNow);

        config.NewConfig<CreateClienteDto, Cliente>()
            .Map(dest => dest.Estado, _ => true);
    }
}

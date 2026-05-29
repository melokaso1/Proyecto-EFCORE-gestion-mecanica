using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class EspecializacionMecanicoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config) =>
        config.NewConfig<EspecializacionMecanico, EspecializacionMecanicoDto>();
}

using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class RepuestoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Repuesto, RepuestoDto>()
            .Map(dest => dest.Categoria, src => src.CategoriaRepuesto!.Nombre);

        config.NewConfig<CreateRepuestoDto, Repuesto>()
            .Map(dest => dest.Activo, _ => true);
    }
}

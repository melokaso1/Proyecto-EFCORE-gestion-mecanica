using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class VehiculoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Vehiculo, VehiculoDto>()
            .Map(dest => dest.Marca, src => src.Modelo!.Marca!.NombreMarca)
            .Map(dest => dest.Modelo, src => src.Modelo!.NombreModelo);

        config.NewConfig<CreateVehiculoDto, Vehiculo>();
    }
}

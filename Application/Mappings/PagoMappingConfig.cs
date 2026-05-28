using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class PagoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Pago, PagoDto>();
    }
}


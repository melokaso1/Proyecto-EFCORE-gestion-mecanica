using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class FacturaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DetalleFactura, DetalleFacturaDto>();

        config.NewConfig<Factura, FacturaDto>()
            .Map(dest => dest.Detalles, src => src.Detalles)
            .Map(dest => dest.Total, src =>
                src.Total > 0
                    ? src.Total
                    : src.ManoObra + src.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario));
    }
}

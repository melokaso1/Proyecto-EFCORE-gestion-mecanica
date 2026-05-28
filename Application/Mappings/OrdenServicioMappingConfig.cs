using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class OrdenServicioMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OrdenServicio, OrdenServicioDto>()
            .Map(dest => dest.VIN, src => src.Vehiculo!.VIN)
            .Map(dest => dest.MarcaModelo, src =>
                src.Vehiculo == null || src.Vehiculo.Modelo == null
                    ? string.Empty
                    : $"{src.Vehiculo.Modelo.Marca!.NombreMarca} {src.Vehiculo.Modelo.NombreModelo}".Trim())
            .Map(dest => dest.Anio, src => src.Vehiculo!.Anio)
            .Map(dest => dest.TipoServicio, src => src.TipoServicio!.Nombre)
            .Map(dest => dest.MecanicoNombre, src =>
                src.Mecanico!.Persona == null
                    ? string.Empty
                    : $"{src.Mecanico.Persona.Nombres} {src.Mecanico.Persona.Apellidos}".Trim())
            .Map(dest => dest.Estado, src => src.EstadoOrden!.Nombre)
            .Ignore(dest => dest.ClienteNombre);
    }
}

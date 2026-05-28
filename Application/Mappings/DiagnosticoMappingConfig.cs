using Application.Dtos;
using Domain.Entities;
using Mapster;

namespace Application.Mappings;

public class DiagnosticoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DiagnosticoOrden, DiagnosticoDto>();
        config.NewConfig<UpsertDiagnosticoDto, DiagnosticoOrden>()
            .Ignore(d => d.IdDiagnosticoOrden)
            .Ignore(d => d.IdOrdenServicio)
            .Ignore(d => d.IdMecanico)
            .Ignore(d => d.FechaDiagnostico);

        config.NewConfig<ReparacionItem, ReparacionItemDto>();
        config.NewConfig<CreateReparacionItemDto, ReparacionItem>()
            .Ignore(r => r.IdReparacionItem)
            .Ignore(r => r.IdOrdenServicio)
            .Ignore(r => r.IdMecanico)
            .Ignore(r => r.Orden)
            .Ignore(r => r.Estado)
            .Ignore(r => r.AprobadoPorJefe)
            .Ignore(r => r.FechaDecisionJefe)
            .Ignore(r => r.ComentarioJefe)
            .Ignore(r => r.AprobadoPorCliente)
            .Ignore(r => r.FechaDecisionCliente)
            .Ignore(r => r.ComentarioCliente)
            .Ignore(r => r.FechaInicio)
            .Ignore(r => r.FechaFin);

        config.NewConfig<UpdateReparacionItemDto, ReparacionItem>()
            .Ignore(r => r.IdReparacionItem)
            .Ignore(r => r.IdOrdenServicio)
            .Ignore(r => r.IdMecanico)
            .Ignore(r => r.Orden)
            .Ignore(r => r.Estado)
            .Ignore(r => r.AprobadoPorJefe)
            .Ignore(r => r.FechaDecisionJefe)
            .Ignore(r => r.ComentarioJefe)
            .Ignore(r => r.AprobadoPorCliente)
            .Ignore(r => r.FechaDecisionCliente)
            .Ignore(r => r.ComentarioCliente)
            .Ignore(r => r.FechaInicio)
            .Ignore(r => r.FechaFin);
    }
}


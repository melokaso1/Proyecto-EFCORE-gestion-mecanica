using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ClienteMappingConfig).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IVehiculoService, VehiculoService>();
        services.AddScoped<IOrdenServicioService, OrdenServicioService>();
        services.AddScoped<IRepuestoService, RepuestoService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        return services;
    }
}

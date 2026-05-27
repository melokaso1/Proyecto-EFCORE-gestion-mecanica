using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AutoTallerDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVehiculoRepository, VehiculoRepository>();
        services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
        services.AddScoped<IRepuestoRepository, RepuestoRepository>();
        services.AddScoped<IDetalleOrdenRepository, DetalleOrdenRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}

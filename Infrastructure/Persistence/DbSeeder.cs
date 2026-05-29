using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

/// <summary>
/// Catálogos mínimos + único super admin. Sin clientes, vehículos ni órdenes de prueba.
/// </summary>
public static class DbSeeder
{
    private const string DominioCorreo = "mecanicas.com";
    private const string CodigoTelefonoDefault = "+57";
    private const string AdminUsuario = "admin";
    private const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        await SeedTiposDocumentoAsync(context, logger);
        await SeedDominioCorreoAsync(context, logger);
        await SeedCodigoTelefonoAsync(context, logger);
        await SeedTiposServicioAsync(context, logger);
        await SeedMarcasModelosAsync(context, logger);
        await SeedSuperAdminAsync(context, logger);
    }

    private static async Task SeedTiposDocumentoAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.TiposDocumento.AnyAsync())
            return;

        context.TiposDocumento.AddRange(
            new TipoDocumento { IdTipoDocumento = 1, Codigo = "CC", Nombre = "Cédula de ciudadanía" },
            new TipoDocumento { IdTipoDocumento = 2, Codigo = "CE", Nombre = "Cédula de extranjería" },
            new TipoDocumento { IdTipoDocumento = 3, Codigo = "PA", Nombre = "Pasaporte" },
            new TipoDocumento { IdTipoDocumento = 4, Codigo = "NI", Nombre = "NIT" });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: tipos de documento creados.");
    }

    private static async Task SeedDominioCorreoAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.DominiosCorreo.AnyAsync(d => d.Dominio == DominioCorreo))
            return;

        context.DominiosCorreo.Add(new DominioCorreo { Dominio = DominioCorreo });
        await context.SaveChangesAsync();
        logger.LogInformation("Seed: dominio de correo {Dominio} creado.", DominioCorreo);
    }

    private static async Task SeedCodigoTelefonoAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.CodigosTelefono.AnyAsync(c => c.Codigo == CodigoTelefonoDefault))
            return;

        context.CodigosTelefono.Add(new CodigoTelefono
        {
            Codigo = CodigoTelefonoDefault,
            Pais = "Colombia"
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: código telefónico {Codigo} creado.", CodigoTelefonoDefault);
    }

    private static async Task SeedTiposServicioAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.TiposServicio.AnyAsync())
            return;

        context.TiposServicio.AddRange(
            new TipoServicio { Nombre = "Mantenimiento preventivo" },
            new TipoServicio { Nombre = "Reparación" },
            new TipoServicio { Nombre = "Diagnóstico" },
            new TipoServicio { Nombre = "Cambio de aceite" },
            new TipoServicio { Nombre = "Alineación y balanceo" });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: tipos de servicio creados.");
    }

    private static async Task SeedMarcasModelosAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.MarcasVehiculo.AnyAsync())
            return;

        var marcas = new[]
        {
            new MarcaVehiculo { IdMarca = 1, NombreMarca = "Toyota" },
            new MarcaVehiculo { IdMarca = 2, NombreMarca = "Chevrolet" },
            new MarcaVehiculo { IdMarca = 3, NombreMarca = "Mazda" },
            new MarcaVehiculo { IdMarca = 4, NombreMarca = "Renault" },
            new MarcaVehiculo { IdMarca = 5, NombreMarca = "Kia" },
            new MarcaVehiculo { IdMarca = 6, NombreMarca = "Nissan" },
            new MarcaVehiculo { IdMarca = 7, NombreMarca = "Hyundai" },
            new MarcaVehiculo { IdMarca = 8, NombreMarca = "Ford" },
            new MarcaVehiculo { IdMarca = 9, NombreMarca = "Volkswagen" },
            new MarcaVehiculo { IdMarca = 10, NombreMarca = "Suzuki" }
        };
        context.MarcasVehiculo.AddRange(marcas);

        context.ModelosVehiculo.AddRange(
            new ModeloVehiculo { IdModelo = 1, IdMarca = 1, IdTipoVehiculo = 1, NombreModelo = "Corolla" },
            new ModeloVehiculo { IdModelo = 2, IdMarca = 1, IdTipoVehiculo = 3, NombreModelo = "RAV4" },
            new ModeloVehiculo { IdModelo = 3, IdMarca = 2, IdTipoVehiculo = 2, NombreModelo = "Spark" },
            new ModeloVehiculo { IdModelo = 4, IdMarca = 2, IdTipoVehiculo = 4, NombreModelo = "D-Max" },
            new ModeloVehiculo { IdModelo = 5, IdMarca = 3, IdTipoVehiculo = 1, NombreModelo = "3" },
            new ModeloVehiculo { IdModelo = 6, IdMarca = 4, IdTipoVehiculo = 2, NombreModelo = "Sandero" },
            new ModeloVehiculo { IdModelo = 7, IdMarca = 5, IdTipoVehiculo = 3, NombreModelo = "Sportage" },
            new ModeloVehiculo { IdModelo = 8, IdMarca = 6, IdTipoVehiculo = 1, NombreModelo = "Sentra" },
            new ModeloVehiculo { IdModelo = 9, IdMarca = 6, IdTipoVehiculo = 2, NombreModelo = "March" },
            new ModeloVehiculo { IdModelo = 10, IdMarca = 7, IdTipoVehiculo = 1, NombreModelo = "Accent" },
            new ModeloVehiculo { IdModelo = 11, IdMarca = 7, IdTipoVehiculo = 3, NombreModelo = "Tucson" },
            new ModeloVehiculo { IdModelo = 12, IdMarca = 8, IdTipoVehiculo = 2, NombreModelo = "Fiesta" },
            new ModeloVehiculo { IdModelo = 13, IdMarca = 8, IdTipoVehiculo = 5, NombreModelo = "Ranger" },
            new ModeloVehiculo { IdModelo = 14, IdMarca = 9, IdTipoVehiculo = 2, NombreModelo = "Gol" },
            new ModeloVehiculo { IdModelo = 15, IdMarca = 9, IdTipoVehiculo = 1, NombreModelo = "Jetta" },
            new ModeloVehiculo { IdModelo = 16, IdMarca = 10, IdTipoVehiculo = 2, NombreModelo = "Swift" },
            new ModeloVehiculo { IdModelo = 17, IdMarca = 10, IdTipoVehiculo = 3, NombreModelo = "Vitara" });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: marcas y modelos de vehículo creados.");
    }

    private static async Task SeedSuperAdminAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.Usuarios.AnyAsync(u => u.Roles.Any(r => r.NombreRol == "Admin")))
            return;

        var dominio = await context.DominiosCorreo.FirstOrDefaultAsync(d => d.Dominio == DominioCorreo);
        var rolAdmin = await context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Admin");
        if (dominio is null || rolAdmin is null)
        {
            logger.LogWarning("Seed: no se pudo crear super admin (dominio o rol Admin ausente).");
            return;
        }

        var persona = new Persona
        {
            Nombres = "Super",
            Apellidos = "Admin",
            FechaRegistro = DateTime.UtcNow
        };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();

        context.CorreosPersona.Add(new CorreoPersona
        {
            IdPersona = persona.IdPersona,
            IdDominioCorreo = dominio.IdDominioCorreo,
            UsuarioCorreo = AdminUsuario,
            EsPrincipal = true
        });

        context.DocumentosPersona.Add(new DocumentoPersona
        {
            IdPersona = persona.IdPersona,
            IdTipoDocumento = 1,
            NumeroDocumento = "1000000001",
            EsPrincipal = true
        });

        var codigo = await context.CodigosTelefono.FirstAsync(c => c.Codigo == CodigoTelefonoDefault);
        context.TelefonosPersona.Add(new TelefonoPersona
        {
            IdPersona = persona.IdPersona,
            IdCodigoTelefono = codigo.IdCodigoTelefono,
            NumeroTelefono = "3000000001",
            EsPrincipal = true
        });

        context.Usuarios.Add(new Usuario
        {
            IdPersona = persona.IdPersona,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminPassword),
            Estado = true,
            Roles = [rolAdmin]
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: super admin {Correo} creado.", $"{AdminUsuario}@{DominioCorreo}");
    }
}

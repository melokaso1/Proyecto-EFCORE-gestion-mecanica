using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    private const string DominioDefault = "jholversito.com";
    private const string CodigoTelefonoDefault = "+57";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        // #region agent log
        Infrastructure.DebugSessionLogger.Log(
            location: "Infrastructure/Persistence/DbSeeder.cs:SeedAsync",
            message: "DbSeeder starting",
            data: new
            {
                database = context.Database.ProviderName,
                canConnect = await context.Database.CanConnectAsync()
            },
            hypothesisId: "H2-seed-startup-fails",
            runId: "pre-fix");
        // #endregion agent log

        await SeedTiposDocumentoAsync(context, logger);
        await SeedDominioCorreoAsync(context, logger);
        await SeedCodigoTelefonoAsync(context, logger);
        await SeedMarcasModelosAsync(context, logger);
        await SeedTiposServicioAsync(context, logger);
        await SeedCategoriasRepuestosAsync(context, logger);
        await SeedUsuariosAsync(context, logger);
        await SeedClientesDemoAsync(context, logger);
        await SeedUsuariosClientesDemoAsync(context, logger);
        await SeedRepuestosDemoAsync(context, logger);
        await SeedVehiculosDemoAsync(context, logger);
        await SeedOrdenesDemoAsync(context, logger);

        // #region agent log
        Infrastructure.DebugSessionLogger.Log(
            location: "Infrastructure/Persistence/DbSeeder.cs:SeedAsync:end",
            message: "DbSeeder completed",
            data: new
            {
                tiposVehiculo = await context.TiposVehiculo.CountAsync(),
                modelosVehiculo = await context.ModelosVehiculo.CountAsync(),
                vehiculos = await context.Vehiculos.CountAsync()
            },
            hypothesisId: "H3-tipo-vehiculo-missing",
            runId: "pre-fix");
        // #endregion agent log
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
        if (await context.DominiosCorreo.AnyAsync(d => d.Dominio == DominioDefault))
            return;

        context.DominiosCorreo.Add(new DominioCorreo { Dominio = DominioDefault });
        await context.SaveChangesAsync();
        logger.LogInformation("Seed: dominio de correo {Dominio} creado.", DominioDefault);
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

    private static async Task SeedMarcasModelosAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.MarcasVehiculo.AnyAsync())
            return;

        var marcas = new (string Marca, string[] Modelos)[]
        {
            ("Toyota", ["Corolla", "Hilux", "RAV4"]),
            ("Chevrolet", ["Spark", "Onix", "D-Max"]),
            ("Mazda", ["3", "CX-5", "BT-50"]),
            ("Renault", ["Logan", "Duster", "Sandero"]),
            ("Hyundai", ["Accent", "Tucson", "Santa Fe"])
        };

        foreach (var (nombreMarca, modelos) in marcas)
        {
            var marca = new MarcaVehiculo { NombreMarca = nombreMarca };
            context.MarcasVehiculo.Add(marca);
            await context.SaveChangesAsync();

            foreach (var nombreModelo in modelos)
            {
                var idTipoVehiculo = InferTipoVehiculoId(nombreModelo);
                context.ModelosVehiculo.Add(new ModeloVehiculo
                {
                    IdMarca = marca.IdMarca,
                    IdTipoVehiculo = idTipoVehiculo,
                    NombreModelo = nombreModelo
                });
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: marcas y modelos de vehículos creados.");
    }

    private static int InferTipoVehiculoId(string modelo) => modelo switch
    {
        "Hilux" or "D-Max" or "BT-50" => 5, // Pickup
        "RAV4" or "CX-5" or "Duster" or "Tucson" or "Santa Fe" => 3, // SUV
        "Spark" or "Onix" or "Sandero" => 2, // Hatchback
        _ => 1 // Sedán (default)
    };

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

    private static async Task SeedCategoriasRepuestosAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.CategoriasRepuesto.AnyAsync())
            return;

        context.CategoriasRepuesto.AddRange(
            new CategoriaRepuesto { Nombre = "Filtros" },
            new CategoriaRepuesto { Nombre = "Frenos" },
            new CategoriaRepuesto { Nombre = "Motor" },
            new CategoriaRepuesto { Nombre = "Suspensión" });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: categorías de repuestos creadas.");
    }

    private static async Task SeedUsuariosAsync(AutoTallerDbContext context, ILogger logger)
    {
        var dominio = await context.DominiosCorreo.FirstOrDefaultAsync(d => d.Dominio == DominioDefault);
        if (dominio is null)
            return;

        await CrearUsuarioSiNoExisteAsync(
            context, dominio, "Admin", "Sistema", "admin", "Admin123!", "Admin", logger);

        await CrearUsuarioSiNoExisteAsync(
            context, dominio, "Laura", "Recepción", "recepcion", "Recep123!", "Recepcionista", logger);

        await CrearUsuarioSiNoExisteAsync(
            context, dominio, "María", "González", "recepcion2", "Recep123!", "Recepcionista", logger);

        await CrearUsuarioSiNoExisteAsync(
            context, dominio, "Pedro", "Herrera", "mecanico1", "Mec123!", "Mecánico", logger);

        await CrearUsuarioSiNoExisteAsync(
            context, dominio, "Diego", "Castro", "mecanico2", "Mec123!", "Mecánico", logger);
    }

    private static async Task CrearUsuarioSiNoExisteAsync(
        AutoTallerDbContext context,
        DominioCorreo dominio,
        string nombres,
        string apellidos,
        string usuarioCorreo,
        string password,
        string nombreRol,
        ILogger logger)
    {
        var correoCompleto = $"{usuarioCorreo}@{dominio.Dominio}";

        var yaExiste = await context.CorreosPersona.AnyAsync(c =>
            c.UsuarioCorreo == usuarioCorreo && c.IdDominioCorreo == dominio.IdDominioCorreo);

        if (yaExiste)
            return;

        var rol = await context.Roles.FirstOrDefaultAsync(r => r.NombreRol == nombreRol);
        if (rol is null)
        {
            logger.LogWarning("Seed: rol {Rol} no encontrado; omitiendo usuario {Correo}.", nombreRol, correoCompleto);
            return;
        }

        var persona = new Persona
        {
            Nombres = nombres,
            Apellidos = apellidos,
            FechaRegistro = DateTime.UtcNow
        };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();

        context.CorreosPersona.Add(new CorreoPersona
        {
            IdPersona = persona.IdPersona,
            IdDominioCorreo = dominio.IdDominioCorreo,
            UsuarioCorreo = usuarioCorreo,
            EsPrincipal = true
        });

        context.DocumentosPersona.Add(new DocumentoPersona
        {
            IdPersona = persona.IdPersona,
            IdTipoDocumento = 1,
            NumeroDocumento = $"1000{persona.IdPersona}",
            EsPrincipal = true
        });

        var codigo = await context.CodigosTelefono.FirstAsync(c => c.Codigo == CodigoTelefonoDefault);
        context.TelefonosPersona.Add(new TelefonoPersona
        {
            IdPersona = persona.IdPersona,
            IdCodigoTelefono = codigo.IdCodigoTelefono,
            NumeroTelefono = $"300{persona.IdPersona:D7}"[..10],
            EsPrincipal = true
        });

        var usuario = new Usuario
        {
            IdPersona = persona.IdPersona,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Estado = true,
            Roles = [rol]
        };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        logger.LogInformation("Seed: usuario {Correo} ({Rol}) creado.", correoCompleto, nombreRol);
    }

    private static async Task SeedClientesDemoAsync(AutoTallerDbContext context, ILogger logger)
    {
        var dominio = await context.DominiosCorreo.FirstOrDefaultAsync(d => d.Dominio == DominioDefault);
        var codigo = await context.CodigosTelefono.FirstOrDefaultAsync(c => c.Codigo == CodigoTelefonoDefault);
        if (dominio is null || codigo is null)
            return;

        var demos = new[]
        {
            ("Carlos", "Méndez", "carlos.mendez", "1012345678", "3001112233"),
            ("Ana", "Torres", "ana.torres", "1023456789", "3002223344"),
            ("Luis", "Ramírez", "luis.ramirez", "1034567890", "3003334455"),
            ("Sofía", "Vargas", "sofia.vargas", "1045678901", "3004445566"),
            ("Jorge", "Silva", "jorge.silva", "1056789012", "3005556677"),
            ("Patricia", "Rojas", "patricia.rojas", "1067890123", "3006667788"),
            ("Andrés", "Muñoz", "andres.munoz", "1078901234", "3007778899")
        };

        var creados = 0;
        foreach (var (nombres, apellidos, usuarioCorreo, documento, telefono) in demos)
        {
            if (await context.DocumentosPersona.AnyAsync(d => d.NumeroDocumento == documento))
                continue;

            var persona = new Persona
            {
                Nombres = nombres,
                Apellidos = apellidos,
                FechaRegistro = DateTime.UtcNow
            };
            context.Personas.Add(persona);
            await context.SaveChangesAsync();

            context.DocumentosPersona.Add(new DocumentoPersona
            {
                IdPersona = persona.IdPersona,
                IdTipoDocumento = 1,
                NumeroDocumento = documento,
                EsPrincipal = true
            });

            context.CorreosPersona.Add(new CorreoPersona
            {
                IdPersona = persona.IdPersona,
                IdDominioCorreo = dominio.IdDominioCorreo,
                UsuarioCorreo = usuarioCorreo,
                EsPrincipal = true
            });

            context.TelefonosPersona.Add(new TelefonoPersona
            {
                IdPersona = persona.IdPersona,
                IdCodigoTelefono = codigo.IdCodigoTelefono,
                NumeroTelefono = telefono,
                EsPrincipal = true
            });

            context.Clientes.Add(new Cliente
            {
                IdPersona = persona.IdPersona,
                Estado = true
            });
            creados++;
        }

        if (creados > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Seed: {Count} clientes de demostración creados.", creados);
        }
    }

    private static async Task SeedUsuariosClientesDemoAsync(AutoTallerDbContext context, ILogger logger)
    {
        var rolCliente = await context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Cliente");
        if (rolCliente is null)
            return;

        // Crear usuarios para los clientes demo (login con correo ya existente).
        // Credenciales demo: password "Cliente123!"
        var clientes = await context.Clientes
            .Include(c => c.Persona!)
            .Where(c => c.Estado)
            .OrderBy(c => c.IdCliente)
            .Take(3)
            .ToListAsync();

        var creados = 0;
        foreach (var cliente in clientes)
        {
            var yaExiste = await context.Usuarios.AnyAsync(u => u.IdPersona == cliente.IdPersona);
            if (yaExiste)
                continue;

            var usuario = new Usuario
            {
                IdPersona = cliente.IdPersona,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
                Estado = true,
                Roles = [rolCliente]
            };
            context.Usuarios.Add(usuario);
            creados++;
        }

        if (creados > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Seed: {Count} usuarios de clientes demo creados (rol Cliente).", creados);
        }
    }

    private static async Task SeedRepuestosDemoAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.Repuestos.AnyAsync())
            return;

        var categorias = await context.CategoriasRepuesto.ToListAsync();
        if (categorias.Count == 0)
            return;

        var catFiltros = categorias.First(c => c.Nombre == "Filtros");
        var catFrenos = categorias.First(c => c.Nombre == "Frenos");
        var catMotor = categorias.First(c => c.Nombre == "Motor");

        context.Repuestos.AddRange(
            new Repuesto
            {
                IdCategoriaRepuesto = catFiltros.IdCategoriaRepuesto,
                Codigo = "FLT-001",
                Descripcion = "Filtro de aceite universal",
                Stock = 45,
                StockMinimo = 10,
                PrecioUnitario = 25000m,
                Activo = true
            },
            new Repuesto
            {
                IdCategoriaRepuesto = catFiltros.IdCategoriaRepuesto,
                Codigo = "FLT-002",
                Descripcion = "Filtro de aire",
                Stock = 30,
                StockMinimo = 8,
                PrecioUnitario = 38000m,
                Activo = true
            },
            new Repuesto
            {
                IdCategoriaRepuesto = catFrenos.IdCategoriaRepuesto,
                Codigo = "FRN-001",
                Descripcion = "Pastillas de freno delanteras",
                Stock = 20,
                StockMinimo = 5,
                PrecioUnitario = 85000m,
                Activo = true
            },
            new Repuesto
            {
                IdCategoriaRepuesto = catMotor.IdCategoriaRepuesto,
                Codigo = "MOT-001",
                Descripcion = "Bujía estándar",
                Stock = 60,
                StockMinimo = 15,
                PrecioUnitario = 12000m,
                Activo = true
            });

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: repuestos de demostración creados.");
    }

    private static async Task SeedVehiculosDemoAsync(AutoTallerDbContext context, ILogger logger)
    {
        var demos = new[]
        {
            ("1HGBH41JXMN109186", 2020, 45000, "1012345678"),
            ("2T1BURHE0JC123456", 2019, 62000, "1023456789"),
            ("3VWDX7AJ5DM123789", 2021, 28000, "1034567890"),
            ("4T1BF1FK5EU123012", 2018, 89000, "1045678901"),
            ("5YJSA1E14HF123345", 2022, 15000, "1056789012"),
            ("6G1ZK5E00CL123678", 2017, 105000, "1067890123"),
            ("7FARW2H85BE123901", 2020, 52000, "1078901234")
        };

        var modelos = await context.ModelosVehiculo.Include(m => m.Marca).ToListAsync();
        if (modelos.Count == 0)
            return;

        var creados = 0;
        for (var i = 0; i < demos.Length; i++)
        {
            var (vin, anio, km, documento) = demos[i];
            if (await context.Vehiculos.AnyAsync(v => v.VIN == vin))
                continue;

            var doc = await context.DocumentosPersona
                .FirstOrDefaultAsync(d => d.NumeroDocumento == documento && d.EsPrincipal);
            if (doc is null)
                continue;

            var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.IdPersona == doc.IdPersona);
            if (cliente is null)
                continue;

            var modelo = modelos[i % modelos.Count];
            var vehiculo = new Vehiculo
            {
                IdModelo = modelo.IdModelo,
                VIN = vin,
                Anio = anio,
                Kilometraje = km
            };
            context.Vehiculos.Add(vehiculo);
            await context.SaveChangesAsync();

            context.HistorialPropietariosVehiculo.Add(new HistorialPropietarioVehiculo
            {
                IdVehiculo = vehiculo.IdVehiculo,
                IdCliente = cliente.IdCliente,
                FechaInicio = DateTime.UtcNow.AddMonths(-6),
                FechaFin = null
            });
            creados++;
        }

        if (creados > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Seed: {Count} vehículos de demostración creados.", creados);
        }
    }

    private static async Task SeedOrdenesDemoAsync(AutoTallerDbContext context, ILogger logger)
    {
        if (await context.OrdenesServicio.AnyAsync())
            return;

        var estados = await context.EstadosOrden.ToDictionaryAsync(e => e.Nombre, e => e.IdEstadoOrden);
        var tipos = await context.TiposServicio.ToListAsync();
        var mecanicos = await context.Usuarios
            .Include(u => u.Roles)
            .Where(u => u.Roles.Any(r => r.NombreRol == "Mecánico"))
            .ToListAsync();
        var vehiculos = await context.Vehiculos.OrderBy(v => v.IdVehiculo).ToListAsync();

        if (mecanicos.Count == 0 || vehiculos.Count == 0 || tipos.Count == 0)
            return;

        var ordenes = new[]
        {
            (0, "Mantenimiento preventivo", EstadosOrden.Pendiente, -2, null as string, (decimal?)180000m, "Cambio de filtros + revisión general", null as bool?),
            (1, "Reparación", EstadosOrden.EnProceso, -5, "Revisión de frenos en curso", (decimal?)420000m, "Pastillas delanteras + rectificado", null as bool?),
            (2, "Diagnóstico", EstadosOrden.Completada, -10, "Diagnóstico eléctrico completado", (decimal?)150000m, "Escaneo y pruebas", true as bool?),
            (3, "Cambio de aceite", EstadosOrden.Pendiente, -1, null, (decimal?)120000m, "Aceite + filtro", null as bool?),
            (4, "Alineación y balanceo", EstadosOrden.EnProceso, -3, "Ajuste de suspensión", (decimal?)90000m, "Alineación + balanceo", null as bool?),
            (5, "Reparación", EstadosOrden.Completada, -15, "Cambio de pastillas de freno", (decimal?)380000m, "Pastillas + mano de obra", true as bool?),
            (6, "Mantenimiento preventivo", EstadosOrden.Pendiente, 0, null, (decimal?)null, null as string, null as bool?)
        };

        for (var i = 0; i < ordenes.Length && i < vehiculos.Count; i++)
        {
            var (vehIdx, tipoNombre, estadoNombre, diasAtras, trabajo, costo, notaCosto, aprobado) = ordenes[i];
            var vehiculo = vehiculos[vehIdx];
            var tipo = tipos.FirstOrDefault(t => t.Nombre == tipoNombre) ?? tipos[0];
            var mecanico = mecanicos[i % mecanicos.Count];

            context.OrdenesServicio.Add(new OrdenServicio
            {
                IdVehiculo = vehiculo.IdVehiculo,
                IdTipoServicio = tipo.IdTipoServicio,
                IdMecanico = mecanico.IdUsuario,
                IdEstadoOrden = estados[estadoNombre],
                FechaIngreso = DateTime.UtcNow.AddDays(diasAtras),
                FechaEstimadaEntrega = DateTime.UtcNow.AddDays(diasAtras + (tipoNombre == "Reparación" ? 3 : 1)),
                TrabajoRealizado = trabajo,
                CostoPropuesto = costo,
                NotaCostoPropuesto = notaCosto,
                CostoAprobado = aprobado,
                FechaDecisionCosto = aprobado is null ? null : DateTime.UtcNow.AddDays(diasAtras + 1),
                ComentarioCliente = aprobado is null ? null : "Aprobado desde el portal"
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seed: órdenes de servicio de demostración creadas.");
    }
}

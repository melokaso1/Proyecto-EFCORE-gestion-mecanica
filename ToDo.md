# TODO — GestionMecanica Backend (ASP.NET Core · Arquitectura Hexagonal)

## FASE 0 — Configuración inicial del proyecto

- [x] 0.1 Crear solución con `dotnet new sln -n GestionMecanica`
- [x] 0.2 Crear proyectos de capas:
  - `dotnet new classlib -n Domain`
  - `dotnet new classlib -n Application`
  - `dotnet new classlib -n Infrastructure`
  - `dotnet new webapi -n API --Use-Controllers`
- [x] 0.3 Agregar todos los proyectos a la solución (`dotnet sln add`)
- [x] 0.4 Referenciar capas en orden:
  - API → Application → Domain
  - Infrastructure → Application
  - API → Infrastructure
- [x] 0.5 Instalar paquetes NuGet por proyecto:
  - **Infrastructure**: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`
  - **Application**: `Mapster`, `Mapster.DependencyInjection`
  - **API**: `Microsoft.AspNetCore.Authentication.JwtBearer`, `AspNetCoreRateLimit`, `Swashbuckle.AspNetCore`

---

## FASE 1 — Capa de Dominio (`Domain`)

### 1.1 Entidades base

- [x] 1.1.1 Crear clase abstracta `BaseEntity` con propiedad `Id` (int)

### 1.2 Entidades del negocio

- [x] 1.2.1 `TipoDocumento` — propiedades: `IdTipoDocumento`, `Codigo` (string, 10), `Nombre` (string, 80)
- [x] 1.2.2 `Persona` — propiedades: `IdPersona`, `Nombres` (100), `Apellidos` (100), `FechaRegistro` (DateTime)
- [x] 1.2.3 `DocumentoPersona` — FK: `IdPersona`, `IdTipoDocumento`; propiedades: `NumeroDocumento` (50), `EsPrincipal` (bool)
- [x] 1.2.4 `DominioCorreo` — propiedades: `IdDominioCorreo`, `Dominio` (100)
- [x] 1.2.5 `CorreoPersona` — FK: `IdPersona`, `IdDominioCorreo`; propiedades: `UsuarioCorreo` (100), `EsPrincipal`
- [x] 1.2.6 `CodigoTelefono` — propiedades: `IdCodigoTelefono`, `Codigo` (10), `Pais` (80)
- [x] 1.2.7 `TelefonoPersona` — FK: `IdPersona`, `IdCodigoTelefono`; propiedades: `NumeroTelefono` (30), `EsPrincipal`
- [x] 1.2.8 `Cliente` — FK: `IdPersona`; propiedades: `IdCliente`, `Estado` (bool)
- [x] 1.2.9 `Usuario` — FK: `IdPersona`; propiedades: `IdUsuario`, `PasswordHash` (255), `Estado` (bool)
- [x] 1.2.10 `Rol` — propiedades: `IdRol`, `NombreRol` (50)
- [x] 1.2.11 Relación N:N `Usuario` ↔ `Rol` — `ICollection<Rol>` en `Usuario` y `ICollection<Usuario>` en `Rol` (tabla intermedia `UsuarioRoles` sin entidad)
- [x] 1.2.12 `MarcaVehiculo` — propiedades: `IdMarca`, `NombreMarca` (80)
- [x] 1.2.13 `ModeloVehiculo` — FK: `IdMarca`; propiedades: `IdModelo`, `NombreModelo` (80)
- [x] 1.2.14 `Vehiculo` — FK: `IdModelo`; propiedades: `IdVehiculo`, `VIN` (17, único), `Anio`, `Kilometraje`
- [x] 1.2.15 `HistorialPropietarioVehiculo` — FK: `IdVehiculo`, `IdCliente`; propiedades: `FechaInicio`, `FechaFin` (nullable)
- [x] 1.2.16 `TipoServicio` — propiedades: `IdTipoServicio`, `Nombre` (80)
- [x] 1.2.17 `EstadoOrden` — propiedades: `IdEstadoOrden`, `Nombre` (50)
- [x] 1.2.18 `OrdenServicio` — FK: `IdVehiculo`, `IdTipoServicio`, `IdMecanico` (Usuario), `IdEstadoOrden`; propiedades: `FechaIngreso`, `FechaEstimadaEntrega` (nullable), `TrabajoRealizado` (Text)
- [x] 1.2.19 `CategoriaRepuesto` — propiedades: `IdCategoriaRepuesto`, `Nombre` (80)
- [x] 1.2.20 `Repuesto` — FK: `IdCategoriaRepuesto`; propiedades: `Codigo` (50, único), `Descripcion` (255), `Stock`, `StockMinimo`, `PrecioUnitario` (decimal 10,2), `Activo`
- [x] 1.2.21 `DetalleOrdenRepuesto` — FK: `IdOrdenServicio`, `IdRepuesto`; propiedades: `Cantidad`, `PrecioUnitarioAplicado` (decimal 10,2); UNIQUE compuesto
- [x] 1.2.22 `Factura` — FK: `IdOrdenServicio` (único); propiedades: `FechaFactura`, `ManoObra` (decimal), `Total` (decimal)
- [x] 1.2.23 `DetalleFactura` — FK: `IdFactura`; propiedades: `Concepto` (150), `Cantidad`, `PrecioUnitario`
- [x] 1.2.24 `TipoAccionAuditoria` — propiedades: `IdTipoAccionAuditoria`, `Nombre` (50)
- [x] 1.2.25 `Auditoria` — FK: `IdUsuario`, `IdTipoAccionAuditoria`; propiedades: `EntidadAfectada` (100), `IdRegistroAfectado`, `FechaHora`, `Descripcion` (Text)

### 1.3 Interfaces de repositorio (Puertos de salida)

- [x] 1.3.1 `IGenericRepository<T>` con métodos:
  - `Task<T?> GetByIdAsync(int id)`
  - `Task<IEnumerable<T>> GetAllAsync()`
  - `Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)`
  - `Task<(IEnumerable<T> items, int total)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null)`
  - `Task AddAsync(T entity)`
  - `void Update(T entity)`
  - `void Remove(T entity)`
- [x] 1.3.2 Interfaces específicas:
  - `IClienteRepository : IGenericRepository<Cliente>`
  - `IVehiculoRepository : IGenericRepository<Vehiculo>`
  - `IOrdenServicioRepository : IGenericRepository<OrdenServicio>`
  - `IRepuestoRepository : IGenericRepository<Repuesto>`
  - `IDetalleOrdenRepository : IGenericRepository<DetalleOrdenRepuesto>`
  - `IFacturaRepository : IGenericRepository<Factura>`
  - `IUsuarioRepository : IGenericRepository<Usuario>`
  - `IAuditoriaRepository : IGenericRepository<Auditoria>`

### 1.4 Reglas de negocio (métodos de dominio)

- [x] 1.4.1 En `OrdenServicio`: método `EstaActiva()` → retorna true si estado no es "Completada" ni "Cancelada"
- [x] 1.4.2 En `Repuesto`: método `TieneStock(int cantidad)` → retorna true si `Stock >= cantidad`
- [x] 1.4.3 En `Vehiculo`: no puede tener dos órdenes simultáneas en estado activo (validar en caso de uso → `OrdenServicioService.CrearOrdenAsync`)

---

## FASE 2 — Capa de Aplicación (`AutoTallerManager.Application`)

### 2.1 DTOs

- [x] 2.1.1 `PersonaDto` — `Nombres`, `Apellidos`, `FechaRegistro`
- [x] 2.1.2 `ClienteDto` — `IdCliente`, `Persona` (PersonaDto), `Estado`
- [x] 2.1.3 `CreateClienteDto` — `Nombres`, `Apellidos`, `Correo`, `Telefono`, `NumeroDocumento`, `IdTipoDocumento`
- [x] 2.1.4 `VehiculoDto` — `IdVehiculo`, `Marca`, `Modelo`, `VIN`, `Anio`, `Kilometraje`
- [x] 2.1.5 `CreateVehiculoDto` — `IdModelo`, `VIN`, `Anio`, `Kilometraje`, `IdCliente`
- [x] 2.1.6 `OrdenServicioDto` — `IdOrdenServicio`, `VIN`, `TipoServicio`, `MecanicoNombre`, `Estado`, `FechaIngreso`, `FechaEstimadaEntrega`, `TrabajoRealizado`
- [x] 2.1.7 `CreateOrdenServicioDto` — `IdVehiculo`, `IdTipoServicio`, `IdMecanico`
- [x] 2.1.8 `UpdateOrdenServicioDto` — `IdEstadoOrden`, `TrabajoRealizado`, `FechaEstimadaEntrega`
- [x] 2.1.9 `RepuestoDto` — `IdRepuesto`, `Codigo`, `Descripcion`, `Stock`, `StockMinimo`, `PrecioUnitario`, `Categoria`, `Activo`
- [x] 2.1.10 `CreateRepuestoDto` — `IdCategoriaRepuesto`, `Codigo`, `Descripcion`, `Stock`, `StockMinimo`, `PrecioUnitario`
- [x] 2.1.11 `DetalleOrdenDto` — `IdRepuesto`, `Descripcion`, `Cantidad`, `PrecioUnitarioAplicado`, `Subtotal`
- [x] 2.1.12 `FacturaDto` — `IdFactura`, `IdOrdenServicio`, `FechaFactura`, `ManoObra`, `Detalles` (List), `Total`
- [x] 2.1.13 `UsuarioDto` — `IdUsuario`, `NombreCompleto`, `Correo`, `Roles`
- [x] 2.1.14 `CreateUsuarioDto` — `Nombres`, `Apellidos`, `Correo`, `Password`, `IdRol`
- [x] 2.1.15 `LoginDto` — `Correo`, `Password`
- [x] 2.1.16 `TokenResponseDto` — `Token`, `Expiracion`
- [x] 2.1.17 `PagedResultDto<T>` — `Items` (List), `TotalCount`, `PageNumber`, `PageSize`

### 2.2 Interfaz de Unit of Work

- [x] 2.2.1 `IUnitOfWork` con propiedades:
  - `IClienteRepository Clientes`
  - `IVehiculoRepository Vehiculos`
  - `IOrdenServicioRepository OrdenesServicio`
  - `IRepuestoRepository Repuestos`
  - `IDetalleOrdenRepository DetallesOrden`
  - `IFacturaRepository Facturas`
  - `IUsuarioRepository Usuarios`
  - `IAuditoriaRepository Auditorias`
  - `Task<int> CommitAsync()`

### 2.3 Configuraciones de Mapster (`IRegister`)

- [x] 2.3.1 `ClienteMappingConfig : IRegister` — mapear `Cliente → ClienteDto`, `CreateClienteDto → Persona + Cliente`
- [x] 2.3.2 `VehiculoMappingConfig : IRegister` — mapear `Vehiculo → VehiculoDto`, `CreateVehiculoDto → Vehiculo`
- [x] 2.3.3 `OrdenServicioMappingConfig : IRegister` — mapear `OrdenServicio → OrdenServicioDto`
- [x] 2.3.4 `RepuestoMappingConfig : IRegister` — mapear `Repuesto → RepuestoDto`, `CreateRepuestoDto → Repuesto`
- [x] 2.3.5 `FacturaMappingConfig : IRegister` — mapear `Factura → FacturaDto`, incluir cálculo de `Total`
- [x] 2.3.6 `UsuarioMappingConfig : IRegister` — mapear `Usuario → UsuarioDto` (ignorar `PasswordHash`)

### 2.4 Interfaces de servicios de aplicación (Puertos de entrada)

- [x] 2.4.1 `IClienteService` con métodos:
  - `Task<ClienteDto> RegistrarClienteConVehiculoAsync(CreateClienteDto dto)`
  - `Task<PagedResultDto<ClienteDto>> ListarClientesAsync(int page, int size, string? filtro)`
  - `Task<ClienteDto?> ObtenerPorIdAsync(int id)`
  - `Task ActualizarAsync(int id, CreateClienteDto dto)`
  - `Task EliminarAsync(int id)` — valida que no tenga órdenes activas
- [x] 2.4.2 `IVehiculoService` con métodos:
  - `Task<VehiculoDto> CrearAsync(CreateVehiculoDto dto)`
  - `Task<PagedResultDto<VehiculoDto>> ListarAsync(int page, int size, int? idCliente, string? vin)`
  - `Task<VehiculoDto?> ObtenerPorVinAsync(string vin)`
  - `Task ActualizarAsync(int id, CreateVehiculoDto dto)`
  - `Task EliminarAsync(int id)` — valida que no tenga órdenes activas
- [x] 2.4.3 `IOrdenServicioService` con métodos:
  - `Task<OrdenServicioDto> CrearOrdenAsync(CreateOrdenServicioDto dto)`
  - `Task ActualizarConTrabajoRealizadoAsync(int id, UpdateOrdenServicioDto dto)`
  - `Task CancelarOrdenAsync(int id)`
  - `Task<PagedResultDto<OrdenServicioDto>> ListarAsync(int page, int size, string? estado, int? idCliente, int? idMecanico, DateTime? desde, DateTime? hasta)`
  - `Task<OrdenServicioDto?> ObtenerPorIdAsync(int id)`
- [x] 2.4.4 `IRepuestoService` con métodos:
  - `Task<RepuestoDto> CrearAsync(CreateRepuestoDto dto)`
  - `Task ActualizarAsync(int id, CreateRepuestoDto dto)`
  - `Task DesactivarAsync(int id)`
  - `Task<PagedResultDto<RepuestoDto>> ListarAsync(int page, int size, string? descripcion, int? idCategoria, bool? soloConStockMinimo)`
  - `Task AjustarStockAsync(int id, int cantidad)` — suma o resta
- [x] 2.4.5 `IFacturaService` con métodos:
  - `Task<FacturaDto> GenerarFacturaAsync(int idOrdenServicio, decimal manoObra)`
  - `Task<FacturaDto?> ObtenerPorOrdenAsync(int idOrdenServicio)`
  - `Task<PagedResultDto<FacturaDto>> ListarAsync(int page, int size, int? idCliente, DateTime? desde, DateTime? hasta)`
- [x] 2.4.6 `IUsuarioService` con métodos:
  - `Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto)`
  - `Task<TokenResponseDto> LoginAsync(LoginDto dto)`
  - `Task<PagedResultDto<UsuarioDto>> ListarAsync(int page, int size)`
  - `Task AsignarRolAsync(int idUsuario, int idRol)`
  - `Task DesactivarAsync(int id)`
- [x] 2.4.7 `IAuditoriaService` con métodos:
  - `Task RegistrarAsync(int idUsuario, string tipoAccion, string entidad, int idRegistro, string? descripcion)`
  - `Task<PagedResultDto<AuditoriaDto>> ListarAsync(int page, int size, string? entidad, int? idUsuario)`

### 2.5 Implementaciones de casos de uso

- [x] 2.5.1 `ClienteService : IClienteService`
  - Usa `IUnitOfWork` y `MapsterMapper.IMapper` (o `.Adapt<T>()`)
  - En `RegistrarClienteConVehiculoAsync`: crear `Persona`, `Cliente`, `DocumentoPersona`, `CorreoPersona`, `TelefonoPersona` y llamar `CommitAsync()` en una sola transacción
  - En `EliminarAsync`: verificar que no haya `OrdenesServicio` con estado activo; lanzar excepción de negocio si existen
- [x] 2.5.2 `VehiculoService : IVehiculoService`
  - Valida unicidad de VIN antes de crear
  - Asigna propietario en `HistorialPropietariosVehiculo` con `FechaInicio = hoy`
  - En `EliminarAsync`: verifica que no haya órdenes activas
- [x] 2.5.3 `OrdenServicioService : IOrdenServicioService`
  - En `CrearOrdenAsync`:
    - Verificar que el vehículo no tenga otra orden activa (estado ≠ Completada/Cancelada)
    - Calcular `FechaEstimadaEntrega` según regla: Mantenimiento +1 día, Reparación +3 días, Diagnóstico +1 día
    - Crear `OrdenServicio` con estado inicial "Pendiente"
    - Registrar auditoría con `IAuditoriaService`
  - En `ActualizarConTrabajoRealizadoAsync`:
    - Actualizar `TrabajoRealizado` y `IdEstadoOrden`
    - Para cada repuesto en `DetallesOrden`: decrementar `Stock` en `Repuestos`
    - Registrar auditoría
- [x] 2.5.4 `RepuestoService : IRepuestoService`
  - En `CrearAsync`: validar que `Codigo` sea único
  - En `AjustarStockAsync`: no permitir stock negativo; lanzar excepción de negocio
- [x] 2.5.5 `FacturaService : IFacturaService`
  - En `GenerarFacturaAsync`:
    - Verificar que la orden esté en estado "Completada"
    - Obtener todos los `DetalleOrdenRepuesto` de la orden
    - Calcular `Total = manoObra + SUM(cantidad * precioUnitarioAplicado)`
    - Crear `Factura` y sus `DetalleFactura` (un registro por repuesto + uno por mano de obra)
    - Llamar `CommitAsync()`
- [x] 2.5.6 `UsuarioService : IUsuarioService`
  - En `CrearAsync`: hashear contraseña con BCrypt (`BCrypt.Net-Next`)
  - En `LoginAsync`:
    - Buscar usuario por correo (join `Persona → CorreoPersona → DominioCorreo`)
    - Verificar hash con BCrypt
    - Generar token JWT con claims: `sub` (IdUsuario), `email`, `role`
    - Retornar `TokenResponseDto`
- [x] 2.5.7 `AuditoriaService : IAuditoriaService`
  - Buscar `TipoAccionAuditoria` por nombre; crear si no existe
  - Persistir `Auditoria` con `CommitAsync()`

### 2.6 Excepciones personalizadas de aplicación

- [x] 2.6.1 `NotFoundException` — se lanza cuando una entidad no existe
- [x] 2.6.2 `BusinessRuleException` — se lanza cuando se viola una regla de negocio
- [x] 2.6.3 `ConflictException` — se lanza por duplicados (VIN, Código repuesto, etc.)

---

## FASE 3 — Capa de Infraestructura (`AutoTallerManager.Infrastructure`)

### 3.1 DbContext y configuración Fluent API

- [x] 3.1.1 Crear `AutoTallerDbContext : DbContext` con todos los `DbSet<T>` para cada entidad
- [x] 3.1.2 Sobrescribir `OnModelCreating` e invocar `modelBuilder.ApplyConfigurationsFromAssembly(...)` para cargar todas las configuraciones
- [x] 3.1.3 Crear clase de configuración para cada entidad usando `IEntityTypeConfiguration<T>`:
  **`TipoDocumentoConfiguration`**
  - Tabla: `TiposDocumento`; PK: `IdTipoDocumento`; `Codigo` unique 10; `Nombre` unique 80
  `**PersonaConfiguration`**
  - Tabla: `Personas`; `Nombres` required 100; `Apellidos` required 100; `FechaRegistro` default `CURRENT_TIMESTAMP`
  `**DocumentoPersonaConfiguration**`
  - Tabla: `DocumentosPersona`; unique compuesto (`IdTipoDocumento`, `NumeroDocumento`); FK `Persona` y `TipoDocumento`
  `**DominioCorreoConfiguration**`
  - Tabla: `DominiosCorreo`; `Dominio` unique 100
  `**CorreoPersonaConfiguration**`
  - Tabla: `CorreosPersona`; unique compuesto (`UsuarioCorreo`, `IdDominioCorreo`); FK `Persona` y `DominioCorreo`
  `**CodigoTelefonoConfiguration**`
  - Tabla: `CodigosTelefono`; `Codigo` unique 10
  `**TelefonoPersonaConfiguration**`
  - Tabla: `TelefonosPersona`; unique compuesto (`IdCodigoTelefono`, `NumeroTelefono`); FK `Persona` y `CodigoTelefono`
  `**ClienteConfiguration**`
  - Tabla: `Clientes`; FK `Persona` (unique); relación 1:1 con `Persona`
  `**UsuarioConfiguration**`
  - Tabla: `Usuarios`; FK `Persona` (unique); `PasswordHash` 255; relación N:N con `Rol` vía tabla intermedia `UsuarioRoles` (sin entidad, `HasMany/WithMany`)
  `**RolConfiguration**`
  - Tabla: `Roles`; `NombreRol` unique 50; relación N:N con `Usuario`
  `**MarcaVehiculoConfiguration**`
  - Tabla: `MarcasVehiculo`; `NombreMarca` unique 80
  `**ModeloVehiculoConfiguration**`
  - Tabla: `ModelosVehiculo`; unique compuesto (`IdMarca`, `NombreModelo`); FK `Marca`
  `**VehiculoConfiguration**`
  - Tabla: `Vehiculos`; `VIN` unique 17; `Anio` tipo YEAR; FK `Modelo`
  `**HistorialPropietarioConfiguration**`
  - Tabla: `HistorialPropietariosVehiculo`; FK `Vehiculo` y `Cliente`; `FechaFin` nullable; DeleteBehavior: `Restrict` en Vehículo y Cliente
  `**TipoServicioConfiguration**`
  - Tabla: `TiposServicio`; `Nombre` unique 80
  `**EstadoOrdenConfiguration**`
  - Tabla: `EstadosOrden`; `Nombre` unique 50
  `**OrdenServicioConfiguration**`
  - Tabla: `OrdenesServicio`; FK: `Vehiculo`, `TipoServicio`, `Mecanico → Usuario`, `EstadoOrden`; `TrabajoRealizado` tipo Text; DeleteBehavior `Restrict` en todos los FK
  `**CategoriaRepuestoConfiguration**`
  - Tabla: `CategoriasRepuesto`; `Nombre` unique 80
  `**RepuestoConfiguration**`
  - Tabla: `Repuestos`; `Codigo` unique 50; `PrecioUnitario` decimal(10,2); `Activo` default true; FK `CategoriaRepuesto`
  `**DetalleOrdenRepuestoConfiguration**`
  - Tabla: `DetalleOrdenRepuestos`; unique compuesto (`IdOrdenServicio`, `IdRepuesto`); `PrecioUnitarioAplicado` decimal(10,2)
  `**FacturaConfiguration**`
  - Tabla: `Facturas`; `IdOrdenServicio` unique (relación 1:1); `ManoObra` y `Total` decimal(10,2)
  `**DetalleFacturaConfiguration**`
  - Tabla: `DetalleFactura`; FK `Factura`; `Concepto` 150; `PrecioUnitario` decimal(10,2)
  `**TipoAccionAuditoriaConfiguration**`
  - Tabla: `TiposAccionAuditoria`; `Nombre` unique 50
  `**AuditoriaConfiguration**`
  - Tabla: `Auditorias`; FK `Usuario` y `TipoAccionAuditoria`; `Descripcion` tipo Text; `FechaHora` default `CURRENT_TIMESTAMP`

### 3.2 Repositorios

- [x] 3.2.1 Implementar `GenericRepository<T> : IGenericRepository<T>` usando `AutoTallerDbContext`
  - `GetByIdAsync` → `FindAsync(id)`
  - `GetAllAsync` → `ToListAsync()`
  - `FindAsync(predicate)` → `Where(predicate).ToListAsync()`
  - `GetPagedAsync` → `Where(filter).Skip((page-1)*size).Take(size).ToListAsync()` + count
  - `AddAsync` → `AddAsync(entity)`
  - `Update` → `Update(entity)`
  - `Remove` → `Remove(entity)`
- [x] 3.2.2 Implementar `ClienteRepository : GenericRepository<Cliente>, IClienteRepository`
  - Sobrescribir `GetAllAsync` para incluir `Include(c => c.Persona)`
  - Método extra: `ExisteConOrdenesActivasAsync(int idCliente)` → join con OrdenesServicio activas
- [x] 3.2.3 Implementar `VehiculoRepository : GenericRepository<Vehiculo>, IVehiculoRepository`
  - `ObtenerPorVinAsync(string vin)` → buscar con include de Modelo y Marca
  - `ExisteConOrdenesActivasAsync(int idVehiculo)`
- [x] 3.2.4 `OrdenServicioRepository : GenericRepository<OrdenServicio>, IOrdenServicioRepository`
  - `GetPagedConFiltrosAsync(...)` → filtrar por estado, idMecanico, fechas, idCliente (via Vehiculo → HistorialPropietarios)
- [x] 3.2.5 `RepuestoRepository : GenericRepository<Repuesto>, IRepuestoRepository`
  - `ListarConFiltrosAsync(...)` → filtrar por categoría, descripción, soloConStockMinimo
- [x] 3.2.6 Implementaciones simples (solo heredan `GenericRepository<T>`):
  - `FacturaRepository`, `UsuarioRepository`, `AuditoriaRepository`, `DetalleOrdenRepository`

### 3.3 Unit of Work

- [x] 3.3.1 Implementar `UnitOfWork : IUnitOfWork`
  - Recibir `AutoTallerDbContext` por inyección
  - Instanciar repositorios lazy o en constructor
  - `CommitAsync()` → `await _context.SaveChangesAsync()`
  - Implementar `IDisposable` → `_context.Dispose()`

### 3.4 Migraciones EF Core

- [x] 3.4.1 Crear migración inicial: `dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API`
- [x] 3.4.2 Verificar que la migración genere todas las tablas de la BD definida
- [x] 3.4.3 Aplicar migración: `dotnet ef database update`
- [x] 3.4.4 Crear migración de datos semilla (`SeedData`): insertar roles "Admin", "Mecánico", "Recepcionista"; insertar estados de orden "Pendiente", "En Proceso", "Completada", "Cancelada"; insertar tipos de acción de auditoría "Creación", "Actualización", "Eliminación"

### 3.5 Registro de dependencias

- [x] 3.5.1 Crear método de extensión `AddInfrastructure(this IServiceCollection services, IConfiguration config)`:
  - Registrar `AutoTallerDbContext` con `UseNpgsql` leyendo cadena de conexión de `appsettings.json`
  - Registrar todos los repositorios: `services.AddScoped<IClienteRepository, ClienteRepository>()` etc.
  - Registrar `IUnitOfWork` → `UnitOfWork`

---

## FASE 4 — Capa de API (`AutoTallerManager.API`)

### 4.1 Configuración de `Program.cs`

- 4.1.1 Registrar `AddInfrastructure(config)` (extensión de infraestructura)
- 4.1.2 Registrar servicios de aplicación:
  - `services.AddScoped<IClienteService, ClienteService>()`
  - `services.AddScoped<IVehiculoService, VehiculoService>()`
  - `services.AddScoped<IOrdenServicioService, OrdenServicioService>()`
  - `services.AddScoped<IRepuestoService, RepuestoService>()`
  - `services.AddScoped<IFacturaService, FacturaService>()`
  - `services.AddScoped<IUsuarioService, UsuarioService>()`
  - `services.AddScoped<IAuditoriaService, AuditoriaService>()`
- 4.1.3 Registrar Mapster: `services.AddMapster(typeof(ClienteMappingConfig).Assembly)`
- 4.1.4 Configurar JWT Authentication:
  - Leer `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` de `appsettings.json`
  - `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)`
  - Validar `Issuer`, `Audience`, `IssuerSigningKey`, `Lifetime`
- 4.1.5 Configurar Autorización con políticas:
  - `services.AddAuthorization(options => { options.AddPolicy("AdminOnly", p => p.RequireRole("Admin")); ... })`
- 4.1.6 Configurar Rate Limiting con `AspNetCoreRateLimit`:
  - Leer reglas de `appsettings.json`
  - Registrar `services.AddMemoryCache()`, `services.AddInMemoryRateLimiting()`, `services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"))`
  - Regla 1: endpoint `/api/ordenesservicio` → 60 req/min
  - Regla 2: endpoint `/api/repuestos` → 30 req/min
  - Respuesta HTTP 429 con mensaje `"Has excedido el límite de solicitudes. Inténtalo en un momento."`
- 4.1.7 Configurar Swagger:
  - Agregar esquema de seguridad `Bearer`
  - Agregar `OperationFilter` para requerir JWT en endpoints con `[Authorize]`
- 4.1.8 Agregar `app.UseIpRateLimiting()`, `app.UseAuthentication()`, `app.UseAuthorization()`
- 4.1.9 Registrar middleware global de manejo de excepciones

### 4.2 Middleware de excepciones

- 4.2.1 Crear `ExceptionMiddleware` que capture:
  - `NotFoundException` → HTTP 404 con mensaje
  - `BusinessRuleException` → HTTP 400 con mensaje
  - `ConflictException` → HTTP 409 con mensaje
  - Cualquier otra excepción → HTTP 500 con mensaje genérico
  - Loggear siempre la excepción completa

### 4.3 Helpers

- 4.3.1 Crear `PaginationHelper` con método estático:
  - `AddPaginationHeader(HttpResponse response, int total, int page, int size)` → agrega `X-Total-Count` al encabezado

### 4.4 Controladores

`**ClientesController**` (`/api/clientes`)

- 4.4.1 `GET /api/clientes?pageNumber=1&pageSize=10&filtro=` → `[Authorize]` → retorna `200` con lista paginada y `X-Total-Count`
- 4.4.2 `GET /api/clientes/{id}` → `[Authorize]` → retorna `200` o `404`
- 4.4.3 `POST /api/clientes` → `[Authorize(Policy="AdminOnly")]` o Recepcionista → retorna `201` con `Location` header
- 4.4.4 `PUT /api/clientes/{id}` → `[Authorize]` → retorna `204` o `404`
- 4.4.5 `DELETE /api/clientes/{id}` → `[Authorize(Policy="AdminOnly")]` → retorna `204` o `400` (si tiene órdenes activas)

`**VehiculosController**` (`/api/vehiculos`)

- 4.4.6 `GET /api/vehiculos?pageNumber=&pageSize=&idCliente=&vin=` → `[Authorize]` → `200` paginado
- 4.4.7 `GET /api/vehiculos/{id}` → `[Authorize]` → `200` o `404`
- 4.4.8 `GET /api/vehiculos/vin/{vin}` → `[Authorize]` → `200` o `404`
- 4.4.9 `POST /api/vehiculos` → `[Authorize(Roles="Admin,Recepcionista")]` → `201`
- 4.4.10 `PUT /api/vehiculos/{id}` → `[Authorize(Roles="Admin,Recepcionista")]` → `204`
- 4.4.11 `DELETE /api/vehiculos/{id}` → `[Authorize(Policy="AdminOnly")]` → `204` o `400`

`**OrdenesServicioController**` (`/api/ordenesservicio`)

- 4.4.12 `GET /api/ordenesservicio?pageNumber=&pageSize=&estado=&idCliente=&idMecanico=&desde=&hasta=` → `[Authorize]` → `200` paginado
- 4.4.13 `GET /api/ordenesservicio/{id}` → `[Authorize]` → `200` o `404`
- 4.4.14 `POST /api/ordenesservicio` → `[Authorize(Roles="Admin,Recepcionista")]` → `201`
- 4.4.15 `PUT /api/ordenesservicio/{id}` → `[Authorize(Roles="Admin,Mecanico")]` → `204`
- 4.4.16 `PATCH /api/ordenesservicio/{id}/cancelar` → `[Authorize(Roles="Admin")]` → `204` o `400`

`**RepuestosController**` (`/api/repuestos`)

- 4.4.17 `GET /api/repuestos?pageNumber=&pageSize=&descripcion=&idCategoria=&soloConStockMinimo=` → `[Authorize]` → `200` paginado
- 4.4.18 `GET /api/repuestos/{id}` → `[Authorize]` → `200` o `404`
- 4.4.19 `POST /api/repuestos` → `[Authorize(Policy="AdminOnly")]` → `201`
- 4.4.20 `PUT /api/repuestos/{id}` → `[Authorize(Policy="AdminOnly")]` → `204`
- 4.4.21 `PATCH /api/repuestos/{id}/stock` con body `{ "cantidad": int }` → `[Authorize(Policy="AdminOnly")]` → `204`
- 4.4.22 `DELETE /api/repuestos/{id}` → `[Authorize(Policy="AdminOnly")]` → desactiva (soft delete) → `204`

`**FacturasController**` (`/api/facturas`)

- 4.4.23 `GET /api/facturas?pageNumber=&pageSize=&idCliente=&desde=&hasta=` → `[Authorize]` → `200` paginado
- 4.4.24 `GET /api/facturas/{id}` → `[Authorize]` → `200` o `404`
- 4.4.25 `GET /api/facturas/orden/{idOrdenServicio}` → `[Authorize]` → `200` o `404`
- 4.4.26 `POST /api/facturas` con body `{ "idOrdenServicio": int, "manoObra": decimal }` → `[Authorize(Roles="Admin,Mecanico")]` → `201`

`**UsuariosController**` (`/api/usuarios`)

- 4.4.27 `POST /api/usuarios/login` → sin autenticación → `200` con `TokenResponseDto` o `401`
- 4.4.28 `GET /api/usuarios?pageNumber=&pageSize=` → `[Authorize(Policy="AdminOnly")]` → `200` paginado
- 4.4.29 `POST /api/usuarios` → `[Authorize(Policy="AdminOnly")]` → `201`
- 4.4.30 `PATCH /api/usuarios/{id}/rol` con body `{ "idRol": int }` → `[Authorize(Policy="AdminOnly")]` → `204`
- 4.4.31 `PATCH /api/usuarios/{id}/desactivar` → `[Authorize(Policy="AdminOnly")]` → `204`

`**AuditoriasController**` (`/api/auditorias`)

- 4.4.32 `GET /api/auditorias?pageNumber=&pageSize=&entidad=&idUsuario=` → `[Authorize(Policy="AdminOnly")]` → `200` paginado

### 4.5 `appsettings.json`

- 4.5.1 Sección `ConnectionStrings:DefaultConnection` con cadena PostgreSQL
- 4.5.2 Sección `Jwt`: `Key`, `Issuer`, `Audience`, `ExpiresInMinutes`
- 4.5.3 Sección `IpRateLimiting` con reglas por endpoint
- 4.5.4 Sección `Logging` estándar

---

## FASE 5 — Documentación Swagger

- 5.1 Configurar `SwaggerGen` con título "AutoTallerManager API", versión "v1", descripción del proyecto
- 5.2 Agregar `SecurityDefinition("Bearer", ...)` con esquema `http`, `bearer`, formato `JWT`
- 5.3 Agregar `SecurityRequirement` global para que todos los endpoints protegidos muestren el candado
- 5.4 Habilitar `app.UseSwagger()` y `app.UseSwaggerUI()` en entorno Development
- 5.5 Agregar comentarios XML en todos los controladores (`/// <summary>`) y habilitar `IncludeXmlComments` en Swagger

---

## FASE 6 — Pruebas manuales con Swagger

- 6.1 Verificar que `POST /api/usuarios/login` retorna token JWT válido
- 6.2 Usar token Bearer en Swagger para probar endpoints protegidos
- 6.3 Verificar HTTP 403 al intentar acceder con rol incorrecto
- 6.4 Verificar HTTP 429 al exceder rate limit en `/api/ordenesservicio`
- 6.5 Verificar `X-Total-Count` en encabezados de listados paginados
- 6.6 Probar flujo completo: crear cliente → crear vehículo → crear orden → actualizar orden → generar factura → consultar auditoría


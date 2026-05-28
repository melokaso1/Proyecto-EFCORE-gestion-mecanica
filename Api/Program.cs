using System.Text;
using Api.Middleware;
using Api.Swagger;
using Application;
using AspNetCoreRateLimit;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// #region agent log
Api.DebugSessionLogger.Log(
    location: "Api/Program.cs:startup",
    message: "API starting",
    data: new
    {
        args = args.Select(a => a.Length > 120 ? a[..120] + "…" : a).ToArray(),
        urlsEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),
        env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    },
    hypothesisId: "H1-port-bind-or-cwd",
    runId: "pre-fix");
// #endregion agent log

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurada.");
var jwtIssuer = configuration["Jwt:Issuer"];
var jwtAudience = configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5249", "https://localhost:7024"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorFrontend", policy => policy
        .WithOrigins(corsOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("X-Total-Count"));
});

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AutoTallerManager API",
        Version = "v1",
        Description = "API REST para gestión de taller mecánico — ASP.NET Core · Arquitectura Hexagonal"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT. Ejemplo: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// #region agent log
Api.DebugSessionLogger.Log(
    location: "Api/Program.cs:built",
    message: "WebApplication built; starting seeding",
    data: new { contentRoot = app.Environment.ContentRootPath },
    hypothesisId: "H2-seed-startup-fails",
    runId: "pre-fix");
// #endregion agent log

await DbSeeder.SeedAsync(app.Services);

// #region agent log
Api.DebugSessionLogger.Log(
    location: "Api/Program.cs:seeded",
    message: "Seeding completed; configuring middleware",
    data: null,
    hypothesisId: "H2-seed-startup-fails",
    runId: "pre-fix");
// #endregion agent log

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoTallerManager API v1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "AutoTallerManager API";
    options.DisplayRequestDuration();
    options.EnablePersistAuthorization();
});

app.UseHttpsRedirection();
app.UseCors("BlazorFrontend");
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .AllowAnonymous();

app.MapControllers();

app.Run();

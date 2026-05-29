using Frontend;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5249");

// In "Production" while running from source (dotnet run --no-launch-profile),
// Static Web Assets may not be enabled by default, which breaks Blazor Server
// script loading (/_framework/blazor.server.js). Enabling it here keeps the app
// runnable regardless of launch profile.
builder.WebHost.UseStaticWebAssets();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    if (builder.Environment.IsDevelopment())
        options.DetailedErrors = true;
});
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IClientesService, ClientesService>();
builder.Services.AddScoped<IVehiculosService, VehiculosService>();
builder.Services.AddScoped<IOrdenesService, OrdenesService>();
builder.Services.AddScoped<ISeguimientoService, SeguimientoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmpleadosService, EmpleadosService>();
builder.Services.AddScoped<IClientePortalService, ClientePortalService>();
builder.Services.AddScoped<IDiagnosticoClientService, DiagnosticoClientService>();
builder.Services.AddScoped<ICajaClientService, CajaClientService>();
builder.Services.AddScoped<ICasosService, CasosService>();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

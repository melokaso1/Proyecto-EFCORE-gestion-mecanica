using Frontend.Models;

namespace Frontend.Services;

public interface IOrdenesService
{
    Task<PagedOrdenesResult> GetMisOrdenesAsync(int page = 1, int size = 20, string? estado = null);
    Task<PagedOrdenesResult> GetOrdenesAsync(int page = 1, int size = 20, string? estado = null);
    Task<OrdenServicioDto?> GetOrdenAsync(int idOrdenServicio);
}

public interface ISeguimientoService
{
    Task<(SeguimientoOrdenDto? Resultado, string? Error)> ConsultarAsync(
        string documento, string? placa, int? codigoOrden);
}

public interface IDashboardService
{
    Task<AdminDashboardDto?> GetAdminAsync();
    Task<RecepcionDashboardDto?> GetRecepcionAsync();
}

public interface IEmpleadosService
{
    Task<PagedUsuariosResult> GetEmpleadosAsync(int page = 1, int size = 50);
}

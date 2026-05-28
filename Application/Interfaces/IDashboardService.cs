using Application.Dtos;

namespace Application.Interfaces;

public interface IDashboardService
{
    Task<AdminDashboardDto> ObtenerAdminAsync();
    Task<RecepcionDashboardDto> ObtenerRecepcionAsync();
}

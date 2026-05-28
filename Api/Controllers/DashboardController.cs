using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Estadísticas y resúmenes por rol.</summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>Resumen para el panel de administración.</summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminDashboardDto>> Admin() =>
        Ok(await dashboardService.ObtenerAdminAsync());

    /// <summary>Resumen para recepción.</summary>
    [HttpGet("recepcion")]
    [Authorize(Roles = "Recepcionista,Admin")]
    public async Task<ActionResult<RecepcionDashboardDto>> Recepcion() =>
        Ok(await dashboardService.ObtenerRecepcionAsync());
}

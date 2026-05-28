using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/jefe-mecanicos")]
[Authorize(Roles = "JefeMecanicos")]
public class JefeMecanicosController(IDiagnosticoService diagnosticoService) : ControllerBase
{
    [HttpGet("pendientes")]
    public async Task<ActionResult<IEnumerable<ReparacionItemDto>>> Pendientes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var items = await diagnosticoService.ListarPendientesJefeAsync(pageNumber, pageSize);
        return Ok(items);
    }
}


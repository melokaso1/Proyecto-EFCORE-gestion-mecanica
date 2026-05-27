using Api.Helpers;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Consulta de registros de auditoría.</summary>
[ApiController]
[Route("api/auditorias")]
[Authorize(Policy = "AdminOnly")]
public class AuditoriasController(IAuditoriaService auditoriaService) : ControllerBase
{
    /// <summary>Lista registros de auditoría paginados.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditoriaDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? entidad = null,
        [FromQuery] int? idUsuario = null)
    {
        var result = await auditoriaService.ListarAsync(pageNumber, pageSize, entidad, idUsuario);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }
}

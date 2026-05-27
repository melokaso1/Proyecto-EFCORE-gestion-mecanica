using Api.Helpers;
using Api.Models;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones CRUD de repuestos.</summary>
[ApiController]
[Route("api/repuestos")]
[Authorize]
public class RepuestosController(IRepuestoService repuestoService) : ControllerBase
{
    /// <summary>Lista repuestos paginados.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepuestoDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? descripcion = null,
        [FromQuery] int? idCategoria = null,
        [FromQuery] bool? soloConStockMinimo = null)
    {
        var result = await repuestoService.ListarAsync(
            pageNumber, pageSize, descripcion, idCategoria, soloConStockMinimo);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Obtiene un repuesto por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RepuestoDto>> Obtener(int id)
    {
        var repuesto = await repuestoService.ObtenerPorIdAsync(id);
        return repuesto is null ? NotFound() : Ok(repuesto);
    }

    /// <summary>Registra un nuevo repuesto.</summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<RepuestoDto>> Crear([FromBody] CreateRepuestoDto dto)
    {
        var repuesto = await repuestoService.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = repuesto.IdRepuesto }, repuesto);
    }

    /// <summary>Actualiza un repuesto.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CreateRepuestoDto dto)
    {
        try
        {
            await repuestoService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Ajusta el stock de un repuesto.</summary>
    [HttpPatch("{id:int}/stock")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AjustarStock(int id, [FromBody] AjustarStockRequest request)
    {
        await repuestoService.AjustarStockAsync(id, request.Cantidad);
        return NoContent();
    }

    /// <summary>Desactiva un repuesto (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Desactivar(int id)
    {
        await repuestoService.DesactivarAsync(id);
        return NoContent();
    }
}

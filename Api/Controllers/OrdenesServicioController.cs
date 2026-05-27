using Api.Helpers;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones de órdenes de servicio.</summary>
[ApiController]
[Route("api/ordenesservicio")]
[Authorize]
public class OrdenesServicioController(IOrdenServicioService ordenServicioService) : ControllerBase
{
    /// <summary>Lista órdenes de servicio paginadas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrdenServicioDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? estado = null,
        [FromQuery] int? idCliente = null,
        [FromQuery] int? idMecanico = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var result = await ordenServicioService.ListarAsync(
            pageNumber, pageSize, estado, idCliente, idMecanico, desde, hasta);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Obtiene una orden de servicio por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrdenServicioDto>> Obtener(int id)
    {
        var orden = await ordenServicioService.ObtenerPorIdAsync(id);
        return orden is null ? NotFound() : Ok(orden);
    }

    /// <summary>Crea una nueva orden de servicio.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    public async Task<ActionResult<OrdenServicioDto>> Crear([FromBody] CreateOrdenServicioDto dto)
    {
        var orden = await ordenServicioService.CrearOrdenAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = orden.IdOrdenServicio }, orden);
    }

    /// <summary>Actualiza trabajo realizado y estado de la orden.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Mecánico")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] UpdateOrdenServicioDto dto)
    {
        try
        {
            await ordenServicioService.ActualizarConTrabajoRealizadoAsync(id, dto);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>Cancela una orden de servicio.</summary>
    [HttpPatch("{id:int}/cancelar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await ordenServicioService.CancelarOrdenAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}

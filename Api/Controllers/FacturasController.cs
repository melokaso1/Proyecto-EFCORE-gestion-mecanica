using Api.Helpers;
using Api.Models;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Operaciones de facturación.</summary>
[ApiController]
[Route("api/facturas")]
[Authorize]
public class FacturasController(IFacturaService facturaService) : ControllerBase
{
    /// <summary>Lista facturas paginadas.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FacturaDto>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? idCliente = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var result = await facturaService.ListarAsync(pageNumber, pageSize, idCliente, desde, hasta);
        PaginationHelper.AddPaginationHeader(Response, result.TotalCount, pageNumber, pageSize);
        return Ok(result.Items);
    }

    /// <summary>Obtiene una factura por id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FacturaDto>> Obtener(int id)
    {
        var factura = await facturaService.ObtenerPorIdAsync(id);
        return factura is null ? NotFound() : Ok(factura);
    }

    /// <summary>Obtiene la factura de una orden de servicio.</summary>
    [HttpGet("orden/{idOrdenServicio:int}")]
    public async Task<ActionResult<FacturaDto>> ObtenerPorOrden(int idOrdenServicio)
    {
        var factura = await facturaService.ObtenerPorOrdenAsync(idOrdenServicio);
        return factura is null ? NotFound() : Ok(factura);
    }

    /// <summary>Genera una factura para una orden completada.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Mecánico")]
    public async Task<ActionResult<FacturaDto>> Generar([FromBody] GenerarFacturaRequest request)
    {
        var factura = await facturaService.GenerarFacturaAsync(request.IdOrdenServicio, request.ManoObra);
        return CreatedAtAction(nameof(Obtener), new { id = factura.IdFactura }, factura);
    }
}

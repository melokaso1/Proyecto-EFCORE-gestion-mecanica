using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>Consulta pública del estado de una orden de servicio.</summary>
[ApiController]
[Route("api/seguimiento")]
public class SeguimientoController(IOrdenServicioService ordenServicioService) : ControllerBase
{
    /// <summary>
    /// Consulta el estado de una orden validando documento del cliente
    /// junto con VIN del vehículo o número de orden.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<SeguimientoOrdenDto>> Consultar(
        [FromQuery] string documento,
        [FromQuery] string? vin = null,
        [FromQuery] int? codigoOrden = null)
    {
        if (string.IsNullOrWhiteSpace(documento))
            return BadRequest(new { message = "El documento del cliente es obligatorio." });

        if (string.IsNullOrWhiteSpace(vin) && codigoOrden is null)
            return BadRequest(new { message = "Indica el VIN del vehículo o el número de orden." });

        var resultado = await ordenServicioService.ConsultarSeguimientoAsync(documento, vin, codigoOrden);
        return resultado is null ? NotFound(new { message = "No se encontró información con esos datos." }) : Ok(resultado);
    }
}

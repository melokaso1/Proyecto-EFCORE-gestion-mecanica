using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/catalogos")]
[Authorize]
public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
{
    [HttpGet("tipos-servicio")]
    public async Task<ActionResult<IEnumerable<TipoServicioDto>>> TiposServicio()
    {
        var items = await catalogoService.ListarTiposServicioAsync();
        return Ok(items);
    }

    [HttpGet("modelos-vehiculo")]
    public async Task<ActionResult<IEnumerable<ModeloVehiculoCatalogoDto>>> ModelosVehiculo()
    {
        var items = await catalogoService.ListarModelosVehiculoAsync();
        return Ok(items);
    }

    [HttpGet("tipos-documento")]
    public async Task<ActionResult<IEnumerable<TipoDocumentoDto>>> TiposDocumento()
    {
        var items = await catalogoService.ListarTiposDocumentoAsync();
        return Ok(items);
    }
}

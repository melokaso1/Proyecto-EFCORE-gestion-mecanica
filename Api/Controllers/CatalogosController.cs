using Application.Dtos;
using Application.Exceptions;
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

    [HttpGet("marcas-vehiculo")]
    public async Task<ActionResult<IEnumerable<MarcaVehiculoDto>>> MarcasVehiculo()
    {
        var items = await catalogoService.ListarMarcasAsync();
        return Ok(items);
    }

    [HttpGet("tipos-vehiculo")]
    public async Task<ActionResult<IEnumerable<TipoVehiculoDto>>> TiposVehiculo()
    {
        var items = await catalogoService.ListarTiposVehiculoAsync();
        return Ok(items);
    }

    [HttpPost("marcas-vehiculo")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MarcaVehiculoDto>> CrearMarca([FromBody] CreateMarcaVehiculoDto dto)
    {
        try
        {
            var marca = await catalogoService.CrearMarcaAsync(dto);
            return CreatedAtAction(nameof(MarcasVehiculo), marca);
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("marcas-vehiculo/{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MarcaVehiculoDto>> ActualizarMarca(int id, [FromBody] CreateMarcaVehiculoDto dto)
    {
        try
        {
            return Ok(await catalogoService.ActualizarMarcaAsync(id, dto));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("marcas-vehiculo/{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> EliminarMarca(int id)
    {
        try
        {
            await catalogoService.EliminarMarcaAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("modelos-vehiculo")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ModeloVehiculoCatalogoDto>> CrearModelo([FromBody] CreateModeloVehiculoDto dto)
    {
        try
        {
            var modelo = await catalogoService.CrearModeloAsync(dto);
            return CreatedAtAction(nameof(ModelosVehiculo), modelo);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("modelos-vehiculo/{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ModeloVehiculoCatalogoDto>> ActualizarModelo(int id, [FromBody] CreateModeloVehiculoDto dto)
    {
        try
        {
            return Ok(await catalogoService.ActualizarModeloAsync(id, dto));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("modelos-vehiculo/{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> EliminarModelo(int id)
    {
        try
        {
            await catalogoService.EliminarModeloAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
